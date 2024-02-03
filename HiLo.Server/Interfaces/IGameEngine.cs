using System.Net.WebSockets;

namespace HiLo.Server.Interfaces
{
    public interface IGameEngine : IDisposable
    {
        public Task StartEngine();

        public Task ConnectPlayer(WebSocket ws);
    }
}
