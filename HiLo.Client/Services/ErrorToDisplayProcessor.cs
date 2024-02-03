using HiLo.Client.Interfaces;
using HiLo.GlobalModels;
using Serilog;

namespace HiLo.Client.Services
{
    public class ErrorToDisplayProcessor : Processor<ErrorToDisplayMessage>, IErrorToDisplayProcessor
    {
        public override void SubProcess(ErrorToDisplayMessage message)
        {
            Log.Information($"Error: {message.Error}");
        }
    }
}
