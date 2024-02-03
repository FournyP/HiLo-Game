using System.Net.WebSockets;

namespace HiLo.Server.Models
{
    public class Player
    {
        public WebSocket WebSocket { get; private set; }

        public Player(WebSocket webSocket) 
        { 
            WebSocket = webSocket;
        }
    }
}
