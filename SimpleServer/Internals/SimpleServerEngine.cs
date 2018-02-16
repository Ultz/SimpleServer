﻿using SimpleServer.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SimpleServer.Internals
{
    public class SimpleServerEngine
    {
        #region Statics
        public static async Task<SimpleServerRequest> ProcessRequestAsync(SimpleServerConnection connection)
        {
            SimpleServerRequest parsedRequest = new SimpleServerRequest();
            var reader = new StreamReader(connection.Stream);
            var request = new StringBuilder();
            string line = null;
            List<string> lines = new List<string>();
            while (!string.IsNullOrWhiteSpace(line = await reader.ReadLineAsync()))
            {
                lines.Add(line);
            }
            var localEndpoint = connection.LocalEndPoint;
            var remoteEnpoint = connection.RemoteEndPoint;
            var headerLines = lines.Skip(1);
            var headers = new Dictionary<string, string>();
            foreach (var headerLine in lines)
            {
                var parts = headerLine.Split(':');
                var key = parts[0];
                var value = parts[1].Trim();
                headers.Add(key, value);
            }
            var rline = lines.ElementAt(0);
            Regex regex = new Regex(RequestRegex);
            Match m = regex.Match(rline);
            if (!m.Success)
            {
                throw new BadRequestException("The request line provided could not be parsed.");
            }
            bool wildcard = false;
            if (!headers.ContainsKey("Host"))
            {
                if (m.Groups["version"].Value == "HTTP/1.1")
                {
                    throw new RfcViolationException("rfc2616-s14.23");
                }
                else if (!connection._server.HasWildcardHost())
                {
                    throw new Exception("404");
                }
                else
                {
                    wildcard = true;
                }
            }
            var url = new UriBuilder(wildcard ? "" : headers["Host"] + m.Groups["url"]).Uri;
            var httpMethod = m.Groups["method"];
            var method = httpMethod.Value;
            //Version = m.Groups["version"].Value;
            //Method = httpMethod.Value;
            //RequestUri = url;
            Stream stream;
            if (method == "POST" || method == "PUT" || method == "PATCH")
            {
                stream = new MemoryStream();
                await reader.BaseStream.CopyToAsync(stream);
            }
            else
            {
                stream = null;
            }
            return new SimpleServerRequest() { Headers = headers, InputStream = stream, LocalEndpoint = connection.LocalEndPoint, Method = method, RemoteEndpoint = connection.RemoteEndPoint, RequestUri = url, Version = m.Groups["version"].Value, Connection = connection };
        }
        #endregion
        public const string RequestRegex = @"^(?<method>GET|HEAD|POST|PUT|DELETE|OPTIONS|TRACE|PATCH).(?<url>.*).(?<version>(HTTP\/1\.1|HTTP\/1\.0))$";

        SimpleServerListener _listener;
        SimpleServer _server;
        SimpleServerHost _host;
        string[] _hostnames;
        public SimpleServerEngine(SimpleServerHost host, SimpleServer server)
        {
            _listener = new SimpleServerListener(new IPEndPoint(host.Endpoint.Scope, host.Endpoint.Port), _server);
            var fqdn = new List<string>() { host.FQDN };
            fqdn.AddRange(host.AliasFQDNs);
            _hostnames = fqdn.ToArray();
            _server = server;
            _host = host;
        }
        public void Start()
        {
            _listener.Start();
        }
        public void Stop()
        {

        }
    }
}