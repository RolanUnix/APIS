using APIS.Enums;

namespace APIS
{
    public class Handler
    {
        public readonly Method Method;
        public readonly string Uri;
        public readonly WebServer.UriHandler MethodHandler;

        public Handler(Method method, string uri, WebServer.UriHandler methodHandler)
        {
            Method = method;
            Uri = uri;
            MethodHandler = methodHandler;
        }
    }
}