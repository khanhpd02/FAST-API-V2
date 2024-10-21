using FAST_API_V2.Middlewares;

namespace FAST_API_V2.ExceptionModel
{
    public class InvalidTokenException : ForbiddenException
    {
        public InvalidTokenException() : base("Invalid token") { }
    }
}
