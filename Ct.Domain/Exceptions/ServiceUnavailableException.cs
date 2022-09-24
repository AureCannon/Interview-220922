namespace Ct.Domain.Exceptions
{
    public sealed class ServiceUnavailableException : Exception
    {
        public ServiceUnavailableException(string message) : base(message)
        {
        }
    }
}
