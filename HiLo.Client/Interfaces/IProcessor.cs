using HiLo.GlobalModels;

namespace HiLo.Client.Interfaces
{
    public interface IProcessor
    {
        public void Process(IMessage message);
    }
}
