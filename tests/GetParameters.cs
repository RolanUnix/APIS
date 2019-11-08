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
    public class GetParameter
    {
        public static void GetParameterTest(string[] args)
        {
            var server = new WebServer(IPAddress.Any, 80);
            server.Start();

            server["/"] = IndexHandler;
        }

        private static Response IndexHandler(Request request)
        {
            var builder = new StringBuilder();

            foreach (var parameter in request.GetParameters)
            {
                builder.Append($"{parameter.Key} = {parameter.Value}<br>");
            }

            return Response.AsHtml(builder.ToString());
        }
    }
}
