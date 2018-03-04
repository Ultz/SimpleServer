﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using SimpleServer.Logging;

namespace SimpleServer.Internals
{
    public class SimpleServerListener : IDisposable
    {
        private CancellationTokenSource _cts;
        private bool _disposed;
        private readonly SimpleServerEngine _engine;
        private bool _isListening;
        private Task _listener;
        private readonly SimpleServer _server;
        private TcpListener _tcpListener;

        public SimpleServerListener(IPEndPoint localEndpoint, SimpleServer server, SimpleServerEngine engine)
        {
            LocalEndpoint = localEndpoint;
            _engine = engine;
            _server = server;
            Initialize();
        }

        public IPEndPoint LocalEndpoint { get; }

        public Socket Socket => _tcpListener.Server;

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<SimpleServerConnection> Accept()
        {
            return Accept_Internal();
        }

        private void Initialize()
        {
            _tcpListener = new TcpListener(LocalEndpoint);
        }

        private async Task<SimpleServerConnection> Accept_Internal()
        {
            var tcpClient = await _tcpListener.AcceptTcpClientAsync();
            return new SimpleServerConnection(tcpClient, _server, this);
        }

        public void Start()
        {
            if (_disposed)
                throw new ObjectDisposedException("Object has been disposed.");

            if (_cts != null)
                throw new InvalidOperationException("HttpListener is already running.");

            _cts = new CancellationTokenSource();
            _isListening = true;
            _tcpListener.Start();
            _listener = Task.Run(Listen, _cts.Token);
        }

        public async Task Listen()
        {
            while (_isListening)
            {
                // Await request.

                var client = await _tcpListener.AcceptTcpClientAsync().ConfigureAwait(false);
                var conn = new SimpleServerConnection(client, _server, this);
                var request = await SimpleServerEngine.ProcessRequestAsync(conn);
                if (request == null)
                    continue;
                // Handle request in a separate thread.

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                Task.Run(async () =>
                {
                    // Process request.

                    var response = new SimpleServerResponse(request, conn);

                    try
                    {
                        await _server.HandleRequestAsync(request, response);
                    }
                    catch (Exception ex)
                    {
                        conn.Dispose();
                        Log.Error(ex);
                    }
                });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
        }

        public void Stop()
        {
            if (_cts == null)
                throw new InvalidOperationException("HttpListener is not running.");

            _cts.Cancel();
            _cts = null;

            _isListening = false;
            _tcpListener.Stop();

            try
            {
                // Stop task
                _listener.Wait(TimeSpan.FromMilliseconds(1));
            }
            catch (Exception)
            {
            }
        }

        public SimpleServerEngine GetEngine()
        {
            return _engine;
        }
    }
}