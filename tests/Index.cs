using System.Net;
using System.Text;
using APIS;
using APIS.Enums;
using APIS.Packets;

namespace Tests
{
    public class Index
    {
        public static void Main(string[] args)
        {
            var server = new WebServer(IPAddress.Any, 80);
            server.Start();

            server[Method.GET, "/"] = IndexHandler;
        }

        private static Response IndexHandler(Request request)
        {
            return Response.AsHtml("Hello");
        }
    }
}