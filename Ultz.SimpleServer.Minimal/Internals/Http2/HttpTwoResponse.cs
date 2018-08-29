﻿#region

using System;
using System.Collections.Generic;
using System.IO;
using Ultz.SimpleServer.Internals.Http;
using Http2.Hpack;
using Http2;

#endregion

namespace Ultz.SimpleServer.Internals.Http2
{
    public class HttpTwoResponse : HttpResponse
    {
        public HttpTwoResponse(HttpRequest request, IStream stream)
        {
            Request = request;
            Stream = stream;
        }

        private HttpRequest Request { get; }
        private IStream Stream { get; }

        public override void Close(bool force = false)
        {
            if (force)
                Stream.CloseAsync().GetAwaiter().GetResult();
            var body = ((MemoryStream) OutputStream).ToArray();
            Stream.WriteHeadersAsync(
                new List<HeaderField> {new HeaderField {Name = ":status", Value = StatusCode.ToString()}},
                body.Length == 0).GetAwaiter().GetResult();
            if (body.Length != 0)
                Stream.WriteAsync(new ArraySegment<byte>(body), true).GetAwaiter().GetResult();
            // the stream is already closed, no need to do anyhting else.
        }
    }
}