using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Net.Mime;
using System.Text;
using System.Web;
using APIS.Enums;
using ContentType = System.Net.Mime.ContentType;
using HttpException = APIS.Exceptions.HttpException;

namespace APIS.Packets
{
    public class Request
    {
        public Method Method;
        public string Uri;
        public string VersionHttp;

        public Dictionary<string, string> GetParameters;
        public Dictionary<string, string> PostParameters;
        public Dictionary<string, string> PatternParameters;

        public Dictionary<string, string> Headers;

        public ContentType ContentType;
        public byte[] Content;

        private Request()
        {
            Uri = string.Empty;
            VersionHttp = string.Empty;

            GetParameters = new Dictionary<string, string>();
            PostParameters = new Dictionary<string, string>();
            Headers = new Dictionary<string, string>();
            PatternParameters = new Dictionary<string, string>();
        }

        public static Request Parse(byte[] httpPacket)
        {
            var request = new Request();

            using (var memoryStream = new MemoryStream(httpPacket))
            {
                var bufferBuilder = new StringBuilder();
                var action = ActionParse.Method;
                var headerKey = string.Empty;
                var content = new List<byte>();

                int symbol;

                do
                {
                    symbol = memoryStream.ReadByte();
                    if (symbol == -1) break;
                    if (symbol == '\r') continue;

                    switch (action)
                    {
                        case ActionParse.Method:
                            if (symbol == ' ')
                            {
                                if (Enum.TryParse(bufferBuilder.ToString().ToUpper(), out request.Method))
                                {
                                    bufferBuilder.Clear();
                                    action = ActionParse.Uri;
                                    break;
                                }

                                throw new HttpException(Code.MethodNotAllowed);
                            }

                            bufferBuilder.Append((char)symbol);

                            break;
                        case ActionParse.Uri:
                            if (symbol == ' ')
                            {
                                request.Uri = HttpUtility.UrlDecode(bufferBuilder.ToString());
                                bufferBuilder.Clear();
                                action = ActionParse.VersionHttp;
                                break;
                            }

                            bufferBuilder.Append((char)symbol);

                            break;
                        case ActionParse.VersionHttp:
                            if (symbol == '\n')
                            {
                                request.VersionHttp = bufferBuilder.ToString();
                                bufferBuilder.Clear();
                                action = ActionParse.HeaderKey;
                                break;
                            }

                            bufferBuilder.Append((char)symbol);

                            break;
                        case ActionParse.HeaderKey:
                            if (symbol == ':')
                            {
                                symbol = memoryStream.ReadByte();
                                if (symbol != ' ') throw new HttpException(Code.BadRequest);
                                headerKey = bufferBuilder.ToString();
                                bufferBuilder.Clear();
                                action = ActionParse.HeaderValue;
                                break;
                            }
                            else if (symbol == '\n')
                            {
                                bufferBuilder.Clear();
                                action = ActionParse.Content;
                                break;
                            }

                            bufferBuilder.Append((char)symbol);

                            break;
                        case ActionParse.HeaderValue:
                            if (symbol == '\n')
                            {
                                request.Headers.Add(headerKey, bufferBuilder.ToString().ToLower());
                                bufferBuilder.Clear();
                                action = ActionParse.HeaderKey;
                                break;
                            }

                            bufferBuilder.Append((char)symbol);

                            break;
                        case ActionParse.Content:
                            content.Add((byte)symbol);
                            break;
                    }
                } while (symbol != -1);

                if (action != ActionParse.Content) throw new HttpException(Code.BadRequest);

                request.Content = content.ToArray();

                try
                {

                    if (request.GetHeader("Content-Type") != null)
                        request.ContentType = new ContentType(request.GetHeader("Content-Type"));
                    else
                        request.ContentType = new ContentType("text/plain");
                }
                catch
                {
                    throw new HttpException(Code.UnsupportedMediaType);
                }

                var splitUri = request.Uri.Split(new [] { '?' }, 2, StringSplitOptions.RemoveEmptyEntries);
                request.Uri = splitUri[0];

                if (splitUri.Length == 2)
                {
                    try
                    {
                        var parameters = splitUri[1].Split(new char[] {'&'}, StringSplitOptions.RemoveEmptyEntries);

                        foreach (var parameter in parameters)
                        {
                            var splitParameter = parameter.Split('=');

                            if (splitParameter.Length == 2)
                            {
                                if (request.GetParameters.ContainsKey(splitParameter[0]))
                                    request.GetParameters[splitParameter[0]] = splitParameter[1];
                                else request.GetParameters.Add(splitParameter[0], splitParameter[1]);
                            }
                            else
                            {
                                throw new Exception();
                            }
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                }

                if (request.Method == Method.POST)
                {
                    if (request.ContentType.MediaType == EnumHelper.GetEnumDescription(EContentType.FormUrlEncoded))
                    {
                        try
                        {
                            var postString = Encoding.UTF8.GetString(content.ToArray());
                            var postParameters = postString.Split('&');
                            foreach (var postParameter in postParameters)
                            {
                                var parameter = postParameter.Split(new[] {'='}, 2);
                                if (parameter.Length == 2)
                                {
                                    request.PostParameters.Add(parameter[0], parameter[1]);
                                }
                                else
                                {
                                    throw new Exception();
                                }
                            }
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                }
            }

            return request;
        }

        public string GetHeader(string key) => Headers.ContainsKey(key.ToLower()) ? Headers[key.ToLower()] : null;
    }
}