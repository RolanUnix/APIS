using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using APIS.Exception;

namespace APIS.Packet
{
    public class Request
    {
        public Method Method;
        public string Uri;
        public string VersionHttp;

        public Dictionary<string, string> Headers;

        public char[] Content;

        private Request()
        {
            Method = Method.Unknown;
            Uri = string.Empty;
            VersionHttp = string.Empty;

            Headers = new Dictionary<string, string>();
        }

        public static Request Parse(byte[] httpPacket)
        {
            var request = new Request();

            using (var memoryStream = new MemoryStream(httpPacket))
            {
                var bufferBuilder = new StringBuilder();
                var action = ActionParse.Method;
                var headerKey = string.Empty;
                var content = new List<char>();

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

                                throw new MethodNotAllowedException();
                            }

                            bufferBuilder.Append((char)symbol);

                            break;
                        case ActionParse.Uri:
                            if (symbol == ' ')
                            {
                                request.Uri = bufferBuilder.ToString();
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
                                if (symbol != ' ') throw new BadRequestException();
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
                                request.Headers.Add(headerKey, bufferBuilder.ToString());
                                bufferBuilder.Clear();
                                action = ActionParse.HeaderKey;
                                break;
                            }

                            bufferBuilder.Append((char)symbol);

                            break;
                        case ActionParse.Content:
                            content.Add((char)symbol);
                            break;
                    }
                } while (symbol != -1);

                if (action != ActionParse.Content) throw new BadRequestException();

                request.Content = content.ToArray();
            }

            return request;
        }
    }
}