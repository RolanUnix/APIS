using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using APIS.Enums;

namespace APIS.Packets
{
    public class Response
    {
        private readonly Code _code;
        private readonly Dictionary<string, string> _headers;
        private readonly byte[] _content;

        private Response(Code code, byte[] content)
        {
            _code = code;
            _headers = new Dictionary<string, string>()
            {
                {"Date", DateTime.Now.ToString("r", CultureInfo.InvariantCulture)},
                {"Server", "APIS"}
            };
            _content = content;
        }

        public static Response AsText(string text)
        {
            return AsCustom(Code.Ok, "text/plain", Encoding.UTF8.GetBytes(text));
        }

        public static Response AsHtml(string text)
        {
            return AsCustom(Code.Ok, "text/html", Encoding.UTF8.GetBytes(text));
        }

        public static Response AsJson(string json)
        {
            return AsCustom(Code.Ok, "application/json", Encoding.UTF8.GetBytes(json));
        }

        public static Response AsRedirect()
        {
            return AsCustom(Code.TemporaryRedirect, "text/html", new byte[0]);
        }

        public static Response AsCustom(Code code, string contentType, byte[] content)
        {
            return new Response(code, content)
                .AddHeader("Content-Type", contentType);
        }

        public byte[] AsHttp()
        {
            _headers["Content-Type"] += "; charset=utf-8";
            AddHeader("Content-Length", _content.Length.ToString());
            var result = Encoding.UTF8.GetBytes("HTTP/1.1 " + (int)_code + " " + EnumHelper.GetEnumDescription(_code) + "\r\n" + string.Join("\r\n", _headers.Select(obj => obj.Key + ": " + obj.Value)) + "\r\n\r\n").ToList();
            result.AddRange(_content);
            return result.ToArray();
        }

        public Response AddHeader(string key, string value = null)
        {
            _headers.Add(key, value);
            return this;
        }
    }
}