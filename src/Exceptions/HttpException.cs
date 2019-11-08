using System.Text;
using APIS.Packets;

namespace APIS.Exceptions
{
    public abstract class HttpException : System.Exception
    {
        private readonly int _errorCode;
        private readonly string _name;
        private readonly string _description;

        public HttpException(int errorCode, string name, string description = null)
        {
            _errorCode = errorCode;
            _name = name;
            _description = description;
        }

        public byte[] AsHttp()
        {
            var content = "<h1>" + _errorCode + " " + _name + "</h1>";
            if (_description != null) content += "<h3>Error:</h3><p>" + _description + "</p><br>";
            content += "<i>Server: APIS</i>";
            return Response.AsCustom(_errorCode, _name, "text/html", Encoding.UTF8.GetBytes(content)).AsHttp();
        }
    }
}
