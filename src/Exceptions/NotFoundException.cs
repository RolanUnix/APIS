namespace APIS.Exceptions
{
    public class NotFoundException : HttpException
    {
        public NotFoundException() : base(404, "Not Found") { }
    }
}