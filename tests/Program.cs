using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using APIS;
using APIS.Enums;
using APIS.Exceptions;
using APIS.Packets;

namespace Tests
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var server = new WebServer(IPAddress.Any, 80);
            server.Start();

            server["/"] = IndexHandler;
        }

        private static Response IndexHandler(Request request)
        {
            return Response.AsHtml("FFF");
        }
    }
}
