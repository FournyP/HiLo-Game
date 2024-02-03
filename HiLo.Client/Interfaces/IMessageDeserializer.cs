using HiLo.GlobalModels;

namespace HiLo.Client.Interfaces
{
    public interface IMessageDeserializer
    {
        public IMessage? Deserialize(ArraySegment<byte> buffer);
    }
}
