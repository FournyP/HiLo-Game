namespace HiLo.GlobalModels
{
    public class GameFinishedMessage : Message
    {
        public int Result { get; set; }

        public int PlayerNumber { get; set; }
    }
}
