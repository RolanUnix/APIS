namespace APIS.Exceptions
{
    public class InternalServerException : HttpException
    {
        public InternalServerException(string error = null) : base(500, "Internal Server Error", error) { }
    }
}