using HiLo.GlobalModels;
using Serilog;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace HiLo.Server.Models
{
    public class Game
    {
        public Guid Id { get; private set; }

        public bool IsStarted { get; private set; }

        public bool IsFinished { get; private set; }

        public int NumberToFind { get; private set; }

        public IList<Round> Rounds { get; private set; }

        public IList<Player> Players { get; private set; }

        public Game(int min, int max) 
        {
            Id = Guid.NewGuid();
            IsStarted = false;
            IsFinished = false;
            Rounds = new List<Round>();
            Players = new List<Player>();

            var rand = new Random();
            NumberToFind = rand.Next(min, max);
        }

        public void Start()
        {
            IsStarted = true;
            Log.Information($"Starting game n°{Id}");
        }

        public void End()
        {
            IsFinished = true;
            Log.Information($"Finish game n°{Id}");
        }

        public void AddPlayer(Player player)
        {
            Players.Add(player);
        }

        public async Task Broadcast<T>(T message) where T : Message
        {
            var jsonMessage = JsonSerializer.Serialize(message);
            
            foreach (var player in Players)
            {
                var buffer = Encoding.UTF8.GetBytes(jsonMessage);

                if (player.WebSocket.State == WebSocketState.Open)
                {
                    await player.WebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }

        public async Task NextRound() 
        {
            if (!IsStarted || IsFinished)
            {
                return;
            }

            Player nextPlayer = FindNextAvailablePlayer();

            if (nextPlayer == null)
            {
                // No player found
                End();
                return;
            }

            var round = new Round(nextPlayer);

            try
            {
                Rounds.Add(round);
                await round.SendRequest();
            } catch (Exception)
            {
                // Sometimes player is unavailable despite the "State" enum in WebSocket class
                await NextRound();
            }
        }

        private Player FindNextAvailablePlayer()
        {
            if (Rounds.Count == 0)
            {
                return Players.First();
            }
             
            var lastRound = Rounds.Last();
            var lastPlayerIndex = Players.IndexOf(lastRound.Player);

            var playerIndex = 0;
            Player nextPlayer = null;

            while (nextPlayer == null && playerIndex < Players.Count())
            {
                var potentialNextPlayerIndex = (lastPlayerIndex + playerIndex + 1) % Players.Count();

                var potentialNextPlayer = Players.ElementAt(potentialNextPlayerIndex);

                if (potentialNextPlayer.WebSocket.State == WebSocketState.Open)
                {
                    nextPlayer = potentialNextPlayer;
                }

                playerIndex++;
            }

            return nextPlayer;
        }
    }
}
