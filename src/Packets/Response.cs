using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace APIS.Packets
{
    public class Response
    {
        private readonly int _code;
        private readonly string _description;
        private readonly Dictionary<string, string> _headers;
        private readonly byte[] _content;

        private Response(int code, string description, byte[] content)
        {
            _code = code;
            _description = description;
            _headers = new Dictionary<string, string>()
            {
                {"Connection", "keep-alive"},
                {"Date", DateTime.Now.ToString("r", CultureInfo.InvariantCulture)},
                {"Server", "APIS"}
            };
            _content = content;
        }

        public static Response AsText(string text)
        {
            return new Response(200, "OK", Encoding.UTF8.GetBytes(text))
                .AddHeader("Content-Type", "text/plain");
        }

        public static Response AsHtml(string text)
        {
            return new Response(200, "OK", Encoding.UTF8.GetBytes(text))
                .AddHeader("Content-Type", "text/html");
        }

        public static Response AsCustom(int code, string description, string contentType, byte[] content)
        {
            return new Response(code, description, content)
                .AddHeader("Content-Type", contentType);
        }

        public byte[] AsHttp()
        {
            _headers["Content-Type"] += "; charset=utf-8";
            AddHeader("Content-Length", _content.Length.ToString());
            var result = Encoding.UTF8.GetBytes("HTTP/1.1 " + _code + " " + _description + "\r\n" + string.Join("\r\n", _headers.Select(obj => obj.Key + ": " + obj.Value)) + "\r\n\r\n").ToList();
            result.AddRange(_content);
            return result.ToArray();
        }

        public Response AddHeader(string key, string value = null)
        {
            _headers.Add(key, value);
            return this;
        }

        private string GetHeader(string key) => _headers.ContainsKey(key) ? _headers[key] : null;
    }
}