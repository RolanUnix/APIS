namespace APIS.Exceptions
{
    public class BadRequestException : HttpException
    {
        public BadRequestException() : base(400, "Bad Request") {}
    }
}