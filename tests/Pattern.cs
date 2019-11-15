using System;
using System.Net;
using System.Text;
using APIS;
using APIS.Enums;
using APIS.Packets;
using ContentType = System.Net.Mime.ContentType;

namespace Tests
{
    public class Pattern
    {
        public static void Main(string[] args)
        {
            var server = new WebServer(IPAddress.Any, 80);
            server.Start();

            server[Method.GET, "/{token}/{action}"] = IndexHandler;

            Console.ReadLine();
        }

        private static Response IndexHandler(Request request)
        {
            var builder = new StringBuilder();

            foreach (var parameter in request.PatternParameters)
            {
                builder.Append($"{parameter.Key} = {parameter.Value}<br>");
            }

            return Response.AsHtml(builder.ToString());
        }
    }
}