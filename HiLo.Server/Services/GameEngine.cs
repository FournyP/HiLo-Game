using HiLo.GlobalModels;
using HiLo.Server.Exceptions;
using HiLo.Server.Interfaces;
using HiLo.Server.Models;
using Serilog;
using System.Net.WebSockets;

namespace HiLo.Server.Services
{
    public class GameEngine : IGameEngine, IDisposable
    {
        private bool _isRuning;

        private readonly Random _rand;

        private readonly IList<Game> _games;

        private readonly object _lockObject = new object();

        public GameEngine() 
        {
            _isRuning = false;
            _rand = new Random();
            _games = new List<Game>();
        }

        public void Dispose()
        {
            _isRuning = false;
        }

        public async Task StartEngine()
        {
            _isRuning = true;

            while(_isRuning)
            {
                IEnumerable<Game> startedGames;

                lock (_lockObject)
                {
                    startedGames = _games.Where(o => o != null && o.IsStarted && !o.IsFinished).ToList();
                }

                foreach(var game in startedGames)
                {
                    if (game.Rounds.Count() == 0)
                    {
                        await game.NextRound();
                    }

                    var currentRound = game.Rounds.Last();
                    var currentPlayerIndex = game.Players.IndexOf(currentRound.Player);

                    try
                    {
                        var isFinished = currentRound.CheckFinish();

                        if (isFinished)
                        {
                            var diff = currentRound.Proposal - game.NumberToFind;

                            byte result = 0;

                            if (diff < 0)
                            {
                                result = 1;
                            }

                            if (diff > 0)
                            {
                                result = 2;
                            }

                            if (result != 0)
                            {
                                var message = new ResultOutputMessage()
                                {
                                    Result = result == 1,
                                    PlayerNumber = currentPlayerIndex,
                                    Proposal = currentRound.Proposal.Value,
                                };

                                await game.Broadcast(message);
                            }
                            else
                            {
                                var message = new GameFinishedMessage()
                                {
                                    PlayerNumber = currentPlayerIndex,
                                    Result = currentRound.Proposal.Value,
                                };

                                await game.Broadcast(message);

                                game.End();

                                lock(_lockObject)
                                {
                                    _games.Remove(game);
                                }
                            }

                            await game.NextRound();
                        }
                    } catch (SubmittedInputException)
                    {
                        var message = new ErrorToDisplayMessage()
                        {
                            Error = $"The player n°{currentPlayerIndex} submit a invalid input"
                        };

                        await game.Broadcast(message);

                        await game.NextRound();
                    }
                    catch (WebSocketClosedException)
                    {
                        var message = new ErrorToDisplayMessage()
                        {
                            Error = $"The player n°{currentPlayerIndex} unexpectedly closed connection"
                        };

                        await game.Broadcast(message);

                        await game.NextRound();
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Error while excuting a game : {ex.Message}");
                        await game.NextRound();
                        continue;
                    }
                }
            }
        }

        public async Task ConnectPlayer(WebSocket ws)
        {
            var unstartedGame = _games.Where(o => o != null && !o.IsStarted).FirstOrDefault();

            if (unstartedGame == null) 
            {
                unstartedGame = CreateGame();
            }
                
            unstartedGame.AddPlayer(new Player(ws));

            var message = new InformationToDisplayMessage()
            {
                Message = $"New connection detected : {unstartedGame.Players.Count()}/2 connected"
            };

            await unstartedGame.Broadcast(message);

            if (unstartedGame.Players.Count == 2)
            {
                message = new InformationToDisplayMessage()
                {
                    Message = $"Enough users, starting the game !"
                };

                await unstartedGame.Broadcast(message);

                unstartedGame.Start();
            }
        }

        private Game CreateGame()
        {
            var maxValue = _rand.Next();
            var minValue = _rand.Next(maxValue);
            var game = new Game(minValue, maxValue);

            lock (_lockObject)
            {
                _games.Add(game);
            }

            return game;
        }
    }
}
