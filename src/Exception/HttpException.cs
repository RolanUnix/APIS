namespace APIS.Exception
{
    public class HttpException : System.Exception
    {
        public int ErrorCode;
        public string Name;

        public HttpException(int errorCode, string name)
        {
            ErrorCode = errorCode;
            Name = name;
        }
    }
}
