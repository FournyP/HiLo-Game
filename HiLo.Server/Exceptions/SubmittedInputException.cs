namespace HiLo.Server.Exceptions
{
    public class SubmittedInputException : Exception
    {
        public SubmittedInputException (Exception baseException) : base($"An exception as been thrown during the deserialization of SubmitInputMessage {baseException.Message}", baseException)
        {

        }
    }
}
