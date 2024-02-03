using HiLo.Client.Exceptions;
using HiLo.Client.Interfaces;
using HiLo.Client.Settings;
using HiLo.GlobalModels;
using Microsoft.Extensions.Options;
using Serilog;
using System.Net.WebSockets;

namespace HiLo.Client
{
    public class App
    {
        private ClientWebSocket _webSocketClient;

        private ExecutionSettings _executionSettings;

        private IMessageDeserializer _messageDeserializer;

        private IDictionary<string, IProcessor> _messageHandlers;

        public App(
            ClientWebSocket webSocketClient,
            IMessageDeserializer messageDeserializer,
            IOptions<ExecutionSettings> executionSettings,
            IResultOutputProcessor resultOutputProcessor,
            IRequestInputProcessor requestInputProcessor,
            IGameFinishedProcessor gameFinishedProcessor,
            IErrorToDisplayProcessor errorToDisplayProcessor,
            IInformationToDisplayProcessor informationToDisplayProcessor)
        {
            _webSocketClient = webSocketClient;
            _messageDeserializer = messageDeserializer;
            _executionSettings = executionSettings.Value;

            _messageHandlers = new Dictionary<string, IProcessor>()
            {
                { "ResultOutputMessage", resultOutputProcessor },
                { "RequestInputMessage", requestInputProcessor },
                { "GameFinishedMessage", gameFinishedProcessor },
                { "ErrorToDisplayMessage", errorToDisplayProcessor },
                { "InformationToDisplayMessage", informationToDisplayProcessor },
            };
        }

        // Async application starting point
        public async Task Run()
        {
            Log.Information($"Trying to connect to {_executionSettings.EngineServerUrl}...");

            await _webSocketClient.ConnectAsync(new Uri(_executionSettings.EngineServerUrl), CancellationToken.None);

            var cts = new CancellationTokenSource();

            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true; // Prevent the process from terminating immediately

                Console.WriteLine("CTRL+C detected. Closing WebSocket connection...");

                cts.Cancel();

                Environment.Exit(0);
            };

            while (true)
            {
                byte[] bufferArray = new byte[4096];
                var buffer = new ArraySegment<byte>(bufferArray);
                await _webSocketClient.ReceiveAsync(buffer, cts.Token);

                try
                {
                    var message = _messageDeserializer.Deserialize(buffer);

                    if (message == null)
                    {
                        throw new InvalidMessageException();
                    } 

                    var process = _messageHandlers[message.GetType().Name];

                    if (process == null)
                    {
                        throw new UnknowTypeException();
                    }

                    process.Process(message);
                } catch (Exception ex)
                {
                    Log.Error($"Error thrown during message handling : {ex.Message}");
                }
            }
        }
    }
}
