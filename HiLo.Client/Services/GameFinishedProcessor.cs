using HiLo.Client.Interfaces;
using HiLo.GlobalModels;
using Serilog;

namespace HiLo.Client.Services
{
    public class GameFinishedProcessor : Processor<GameFinishedMessage>, IGameFinishedProcessor
    {
        public override void SubProcess(GameFinishedMessage message)
        {
            Log.Information($"Congratulation ! Player n°{message.PlayerNumber} found the result. It was : {message.Result} !");
        }
    }
}
