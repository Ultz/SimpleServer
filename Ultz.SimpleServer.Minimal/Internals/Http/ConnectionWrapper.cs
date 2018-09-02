﻿#region

using System;
using System.IO;
using System.Threading.Tasks;
using Http2;

#endregion

namespace Ultz.SimpleServer.Internals.Http
{
    internal class ConnectionByteStream : IReadableByteStream, IWriteAndCloseableByteStream
    {
        public ConnectionByteStream(IConnection connection)
        {
            Memory = new MemoryStream();
            IsUsingMemory = false;
            Connection = connection;
        }

        public IConnection Connection { get; }
        public MemoryStream Memory { get; }
        public bool IsUsingMemory { get; set; }

        public async ValueTask<StreamReadResult> ReadAsync(ArraySegment<byte> buffer)
        {
            var stream = IsUsingMemory ? Memory : Connection.Stream;
            var res = await stream.ReadAsync(buffer.Array, buffer.Offset, buffer.Count);
            await Memory.WriteAsync(buffer.Array, buffer.Offset, buffer.Count);
            return new StreamReadResult
            {
                BytesRead = res,
                EndOfStream = res == 0
            };
        }

        public async Task WriteAsync(ArraySegment<byte> buffer)
        {
            await Connection.Stream.WriteAsync(buffer.Array, buffer.Offset, buffer.Count);
        }

        public Task CloseAsync()
        {
            Connection.Close();
            Memory.Dispose();
            return Task.CompletedTask;
        }
    }
}