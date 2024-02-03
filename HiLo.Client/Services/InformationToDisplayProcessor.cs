using HiLo.Client.Interfaces;
using HiLo.GlobalModels;
using Serilog;

namespace HiLo.Client.Services
{
    public class InformationToDisplayProcessor : Processor<InformationToDisplayMessage>, IInformationToDisplayProcessor
    {
        public override void SubProcess(InformationToDisplayMessage message)
        {
            Log.Information($"Information: {message.Message}");
        }
    }
}
