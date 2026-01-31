namespace Exceptions
{
    public class InvalidPersonIdException : ArgumentException
    {
        public InvalidPersonIdException() : base("The provided person ID is invalid.")
        {
        }

        public InvalidPersonIdException(string? message) : base(message)
        {
        }

        public InvalidPersonIdException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

    }
}
