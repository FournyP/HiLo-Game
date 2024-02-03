using HiLo.Client.Interfaces;
using HiLo.GlobalModels;
using Serilog;

namespace HiLo.Client.Services
{
    public class ResultOutputProcessor : Processor<ResultOutputMessage>, IResultOutputProcessor
    {
        public override void SubProcess(ResultOutputMessage message)
        {
            var result = "LO";
            if (message.Result)
            {
                result = "HI";
            }

            Log.Information($"Player n°{message.PlayerNumber} submit : {message.Proposal}. And the result is : {result}");
        }
    }
}
