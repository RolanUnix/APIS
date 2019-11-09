using System.Collections.Generic;
using System.Text.RegularExpressions;
using APIS.Enums;

namespace APIS
{
    public class Handler
    {
        public readonly Method Method;
        public readonly WebServer.UriHandler MethodHandler;
        public readonly Regex UriRegex;
        public readonly List<string> Parameters;

        public Handler(Method method, string uri, WebServer.UriHandler methodHandler)
        {
            Method = method;
            Parameters = new List<string>();

            var regex = new Regex(@"{\w+}", RegexOptions.Compiled);

            UriRegex = new Regex("^" + regex.Replace(uri, @"(\w*)") + "$");

            var groupCollection = regex.Matches(uri);

            foreach (Match match in groupCollection)
            {
                var value = match.Groups[0].Value;
                Parameters.Add(value.Substring(1, value.Length - 2));
            }

            MethodHandler = methodHandler;
        }
    }
}