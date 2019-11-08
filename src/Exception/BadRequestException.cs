namespace APIS.Exception
{
    public class BadRequestException : HttpException
    {
        public BadRequestException() : base(400, "Bad Request") {}
    }
}