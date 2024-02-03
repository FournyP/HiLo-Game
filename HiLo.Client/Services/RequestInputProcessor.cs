using HiLo.Client.Interfaces;
using HiLo.GlobalModels;
using Serilog;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace HiLo.Client.Services
{
    public class RequestInputProcessor : Processor<RequestInputMessage>, IRequestInputProcessor
    {
        private readonly ClientWebSocket _webSocketClient;

        public RequestInputProcessor(ClientWebSocket webSocketClient) 
        {
            _webSocketClient = webSocketClient;
        }

        public override void SubProcess(RequestInputMessage message)
        {
            Log.Information("You have been selected to try a number ! Please input one");
            var strResult = Console.ReadLine();

            if (strResult == null)
            {
                Process(message);
                return;
            }

            try
            {
                var value = int.Parse(strResult);
                var result = new SubmitInputMessage()
                {
                    Value = value,
                };

                var jsonResult = JsonSerializer.Serialize(result);
                var buffer = Encoding.UTF8.GetBytes(jsonResult);

                _webSocketClient.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None).Wait();
            }
            catch (Exception)
            {
                Process(message);
            }
        }
    }
}
