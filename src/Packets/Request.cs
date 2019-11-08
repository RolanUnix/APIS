﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Text;
using APIS.Enums;
using APIS.Exceptions;

namespace APIS.Packets
{
    public class Request
    {
        public Method Method;
        public string Uri;
        public string VersionHttp;

        public Dictionary<string, string> GetParameters;

        public Dictionary<string, string> Headers;

        public char[] Content;

        private Request()
        {
            Uri = string.Empty;
            VersionHttp = string.Empty;

            Headers = new Dictionary<string, string>();
            GetParameters = new Dictionary<string, string>();
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

                                throw new HttpException(Code.MethodNotAllowed);
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

                if (action != ActionParse.Content) throw new HttpException(Code.BadRequest);

                request.Content = content.ToArray();

                var splitUri = request.Uri.Split(new [] { '?' }, 2, StringSplitOptions.RemoveEmptyEntries);
                
                if (splitUri.Length == 2)
                {
                    var parameters = splitUri[1].Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
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
                            throw new HttpException(Code.BadRequest);
                        }
                    }

                    request.Uri = splitUri[0];
                }
            }

            return request;
        }
    }
}