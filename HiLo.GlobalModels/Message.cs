namespace HiLo.GlobalModels
{
    abstract public class Message : IMessage
    {
        public string Type { get; set; }

        public Message()
        {
            Type = GetType().Name;
        }
    }
}
