﻿using System;
using System.Net;
using SimpleServer;
using SimpleServer.Logging;
using SS = SimpleServer.SimpleServer;

namespace TestApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Starting test app...");
            SS.Initialize();
            Log.AddWriter(Console.Out);
            //SS server = new SS();
            //SimpleServerHost host = new SimpleServerHost();
            //host.Handlers.Add(new TestHandler());
            //host.Endpoint = new SimpleServerEndpoint() { Port = 11111, Scope = IPAddress.Loopback };
            //server.Hosts.Add(host);
            var server = ServerBuilder.NewServer().NewHost(11111).For(IPAddress.Loopback)
                .With(new TestHandler()).AddToServer().Build();
            server.Start();
            Console.ReadLine();
        }
    }
}