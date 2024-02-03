using HiLo.GlobalModels;
using HiLo.Server.Exceptions;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace HiLo.Server.Models
{
    public class Round
    {
        public int? Proposal { get; private set; }

        public Player Player { get; private set; }
    
        public bool IsFinished { get; private set; }

        public DateTime StartedAt { get; private set; }

        private ArraySegment<byte> _buffer;

        private Task<WebSocketReceiveResult> _webSocketReceiveResult;

        public Round(Player player) 
        { 
            Player = player;
            IsFinished = false;
            StartedAt = DateTime.Now;
        }

        public async Task SendRequest()
        {
            if (_webSocketReceiveResult != null || Player.WebSocket.State == WebSocketState.Closed)
            {
                throw new WebSocketClosedException();
            }

            var ws = Player.WebSocket;

            var message = new RequestInputMessage();
            var buffer = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            await ws.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);

            byte[] bufferArray = new byte[4096];
            _buffer = new ArraySegment<byte>(bufferArray);
            _webSocketReceiveResult = ws.ReceiveAsync(_buffer, CancellationToken.None);
        }

        public bool CheckFinish()
        {
            if (IsFinished)
            {
                throw new RoundAlreadyChecked();
            }

            if (_webSocketReceiveResult.IsCompletedSuccessfully)
            {
                IsFinished = true;

                try
                {
                    _buffer = _buffer.Where(o => o != 0).ToArray();
                    var json = Encoding.UTF8.GetString(_buffer.Array, _buffer.Offset, _buffer.Count);
                    var submitInputMessage = JsonSerializer.Deserialize<SubmitInputMessage>(json);

                    Proposal = submitInputMessage.Value;
                }
                catch (Exception ex)
                {
                    throw new SubmittedInputException(ex);
                }
            }

            if (_webSocketReceiveResult.IsCanceled || _webSocketReceiveResult.IsFaulted || Player.WebSocket.State == WebSocketState.Closed)
            {
                throw new WebSocketClosedException();
            }

            return IsFinished;
        }
    }
}
