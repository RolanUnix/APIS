using System;
using System.Text;
using APIS.Enums;
using APIS.Packets;

namespace APIS.Exceptions
{
    public class HttpException : System.Exception
    {
        private readonly Code _code;
        private readonly string _description;

        public HttpException(Code code, string description = null)
        {
            _code = code;
            _description = description;
        }

        public byte[] AsHttp()
        {
            var content = "<h1>" + (int)_code + " " + EnumHelper.GetEnumDescription(_code) + "</h1>";
            if (_description != null) content += "<h3>Error:</h3><p>" + _description + "</p><br>";
            content += "<i>Server: APIS</i>";
            return Response.AsCustom(_code, "text/html", Encoding.UTF8.GetBytes(content)).AsHttp();
        }
    }
}
