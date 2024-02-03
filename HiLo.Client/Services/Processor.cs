using HiLo.GlobalModels;

namespace HiLo.Client.Services
{
    public abstract class Processor<T> where T : Message
    {
        public void Process(IMessage message)
        {
            var typedMessage = (T) message;
            SubProcess(typedMessage);
        }

        public abstract void SubProcess(T message);
    }
}
