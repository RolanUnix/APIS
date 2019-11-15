using System.Net;
using System.Text;
using APIS;
using APIS.Enums;
using APIS.Packets;

namespace Tests
{
    public class PostParameters
    {
        public static void Main(string[] args)
        {
            var server = new WebServer(IPAddress.Any, 80);
            server.Start();

            server[Method.POST, "/"] = IndexHandler;
        }

        private static Response IndexHandler(Request request)
        {
            var builder = new StringBuilder();

            foreach (var parameter in request.PostParameters)
            {
                builder.Append($"{parameter.Key} = {parameter.Value}<br>");
            }

            builder.Append(Encoding.UTF8.GetString(request.Content));
            builder.Append("<br>" + request.ContentType.Boundary);

            return Response.AsHtml(builder.ToString());
        }
    }
}