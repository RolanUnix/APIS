using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using APIS.Enums;
using APIS.Exceptions;
using APIS.Packets;

namespace APIS
{
    public class WebServer
    {
        public delegate Response UriHandler(Request request);

        private readonly List<Handler> _handlers;
        private readonly IPAddress _bindAddress;
        private readonly int _port;
        private readonly bool _multiThreading;
        private readonly bool _debug;
        private bool _serverStatus;
        private Thread _serverThread;

        private TcpListener _serverListener;

        public UriHandler this[Method method, string uri]
        {
            private get => _handlers.Any(obj => obj.Method == method && obj.UriRegex.IsMatch(uri)) ? _handlers.First(obj => obj.Method == method && obj.UriRegex.IsMatch(uri)).MethodHandler : null;

            set
            {
                if (this[method, uri] != null) throw new Exception("Handler already exists");
                _handlers.Add(new Handler(method, uri, value));
            }
        }

        public WebServer(IPAddress bindAddress, int port, bool multiThreading = true, bool debug = true)
        {
            _handlers = new List<Handler>();
            _bindAddress = bindAddress;
            _port = port;
            _multiThreading = multiThreading;
            _serverStatus = false;
            _debug = debug;
        }

        public void Start()
        {
            _serverStatus = true;

            _serverListener = new TcpListener(_bindAddress, _port);
            _serverListener.Start();

            _serverThread = new Thread(HandlerServer);
            _serverThread.Start();
        }

        public void Stop()
        {
            _serverThread.Abort();
            _serverStatus = false;
            _serverListener.Stop();
        }

        private void HandlerServer()
        {
            while (_serverStatus)
            {
                try
                {
                    var client = _serverListener.AcceptTcpClient();

                    if (_multiThreading)
                    {
                        new Thread(_ => HandlerClient(client)).Start();
                    }
                    else
                    {
                        HandlerClient(client);
                    }
                }
                catch
                {
                    // ignored
                }
            }
        }

        private void HandlerClient(TcpClient client)
        {
            do
            {
                try
                {
                    var buffer = new byte[8 * 1024 * 1024];
                    var bytesAccepted = client.Client.Receive(buffer);

                    buffer = buffer.Take(bytesAccepted).ToArray();

                    if (buffer.Length == 0) continue;

                    var packet = Request.Parse(buffer);

                    var methodHandler = this[packet.Method, packet.Uri];

                    if (methodHandler == null) throw new HttpException(Code.NotFound);

                    var handler = _handlers.First(obj => obj.MethodHandler == methodHandler);

                    var matches = handler.UriRegex.Match(packet.Uri);

                    for (var i = 0; i < handler.Parameters.Count; i++)
                    {
                        packet.PatternParameters.Add(handler.Parameters[i], matches.Groups[1 + i].Value);
                    }

                    client.Client.Send(methodHandler.Invoke(packet).AsHttp());
                }
                catch (Exception e)
                {
                    HttpException error;

                    if (e is HttpException) error = (HttpException)e;
                    else error = new HttpException(Code.InternalServerError, _debug ? e.ToString() : null);

                    if (client.Connected)
                    {
                        client.Client.Send(error.AsHttp());
                    }
                }
            } while (_multiThreading && _serverStatus && client.Connected);

            client.Close();
        }
    }
}
