namespace HiLo.GlobalModels
{
    public class ResultOutputMessage : Message
    {
        /// <summary>
        /// true: proposal < number
        /// false: proposal > number
        /// </summary>
        public bool Result { get; set; }

        public int Proposal { get; set; }

        public int PlayerNumber { get; set; }
    }
}
