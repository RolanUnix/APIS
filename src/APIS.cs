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

        private readonly Dictionary<string, UriHandler> _handlers;
        private readonly IPAddress _bindAddress;
        private readonly int _port;
        private readonly bool _multiThreading;
        private readonly bool _debug;
        private bool _serverStatus;
        private Thread _serverThread;

        private TcpListener _serverListener;

        public UriHandler this[string uri]
        {
            get => _handlers.ContainsKey(uri) ? _handlers[uri] : null;

            set
            {
                if (_handlers.ContainsKey(uri)) _handlers[uri] = value;
                else _handlers.Add(uri, value);
            }
        }

        public WebServer(IPAddress bindAddress, int port, bool multiThreading = true, bool debug = false)
        {
            _handlers = new Dictionary<string, UriHandler>();
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

                    var handler = this[packet.Uri];

                    if (handler == null) throw new HttpException(Code.NotFound);

                    client.Client.Send(handler.Invoke(packet).AsHttp());
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
