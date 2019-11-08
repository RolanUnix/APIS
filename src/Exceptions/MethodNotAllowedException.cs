namespace APIS.Exceptions
{
    public class MethodNotAllowedException : HttpException
    {
        public MethodNotAllowedException() : base(405, "Method Not Allowed") {}
    }
}