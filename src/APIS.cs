using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using APIS.Packet;

namespace APIS
{
    public class WebServer
    {
        public delegate Response UriHandler(Request request);

        private Dictionary<string, UriHandler> _handlers;
        private readonly IPAddress _bindAddress;
        private readonly int _port;
        private readonly bool _multiThreading;
        private readonly bool _serverStatus;
        private readonly object _queueLocker;
        private Thread _serverThread;
        private Thread _queueThread;
        private Queue _queueRequests;

        private TcpListener _serverListener;

        public UriHandler this[string uri]
        {
            set
            {
                if (_handlers.ContainsKey(uri)) _handlers[uri] = value;
                else _handlers.Add(uri, value);
            }
        }

        public WebServer(IPAddress bindAddress, int port, bool multiThreading = true)
        {
            _bindAddress = bindAddress;
            _port = port;
            _multiThreading = multiThreading;
            _serverStatus = false;
            if (!multiThreading)
            {
                _queueRequests = new Queue();
                _queueLocker = new object();
            }
        }

        public void Start()
        {
            if (!_multiThreading)
            {
                _queueThread = new Thread(HandlerQueue);
                _queueThread.Start();
            }

            _serverListener = new TcpListener(_bindAddress, _port);
            _serverListener.Start();

            _serverThread = new Thread(HandlerServer);
            _serverThread.Start();
        }

        private void HandlerServer()
        {
            while (_serverStatus)
            {
                try
                {

                }
                catch (Exception e)
                {

                }
            }
        }

        private void HandlerQueue()
        {
            while (_serverStatus)
            {
                Thread.Sleep(1);
            }
        }
    }
}