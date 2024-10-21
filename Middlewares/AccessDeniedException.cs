namespace FAST_API_V2.Middlewares
{
    public class AccessDeniedException : ForbiddenException
    {
        public AccessDeniedException() : base("Access denied") { }
    }
}
