using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using APIS;
using APIS.Packets;

namespace Tests
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var server = new WebServer(IPAddress.Any, 8000);
            server.Start();

            server["/"] = IndexHandler;

            Console.ReadLine();
        }

        private static Response IndexHandler(Request request)
        {
            return Response.AsHtml("<h1>Hello World</h1>");
        }
    }
}
