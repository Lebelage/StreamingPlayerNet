using LibVLCSharp.Shared;
using MonoTorrent.Streaming;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace StreamingPlayer.Core.TorrentEngine
{
    public class BufferedStream : Stream
    {
        private readonly Stream _baseStream;
        private readonly Task _prefetchTask;
        private readonly CancellationTokenSource _cts = new();
        private readonly object _lock = new();

        private long _position;
        private readonly byte[] _buffer;
        private long _bufferStart;
        private int _bufferValid;
        private const int BufferSize = 100 * 1024 * 1024; 

        public override bool CanRead => true;
        public override bool CanSeek => true;
        public override bool CanWrite => false;
        public override long Length => _baseStream.Length;
        public override long Position
        {
            get => _position;
            set => Seek(value, SeekOrigin.Begin);
        }

        public BufferedStream(Stream baseStream)
        {
            _baseStream = baseStream ?? throw new ArgumentNullException(nameof(baseStream));
            _buffer = new byte[BufferSize];
            _bufferStart = 0;
            _bufferValid = 0;
            _prefetchTask = Task.Run(PrefetchLoop);
        }

        private async void PrefetchLoop()
        {
            try
            {
                while (!_cts.IsCancellationRequested)
                {
                    await Task.Delay(15, _cts.Token).ConfigureAwait(false);

                    lock (_lock)
                    {
                        long bufferedAhead = _bufferStart + _bufferValid - _position;
                        if (bufferedAhead >= BufferSize * 0.95) continue;

                        long readFrom = _bufferStart + _bufferValid;
                        if (readFrom >= _baseStream.Length) continue;

                        int toRead = (int)Math.Min(BufferSize - _bufferValid, _baseStream.Length - readFrom);

                        var temp = new byte[toRead];
                        int read = 0;
                        try
                        {
                            long oldPos = _baseStream.Position;
                            _baseStream.Position = readFrom;
                            read = _baseStream.Read(temp, 0, toRead);
                            _baseStream.Position = oldPos;
                        }
                        catch { }

                        if (read > 0)
                        {
                            Buffer.BlockCopy(temp, 0, _buffer, _bufferValid, read);
                            _bufferValid += read;
                        }
                    }
                }
            }
            catch { }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int totalRead = 0;

            int fromBuffer = 0;
            int fromStream = 0;

            while (totalRead < count)
            {
                lock (_lock)
                {
                    long bufferPos = _position - _bufferStart;
                    if (bufferPos >= 0 && bufferPos < _bufferValid)
                    {
                        int available = (int)(_bufferValid - bufferPos);
                        int toCopy = Math.Min(available, count - totalRead);
                        Buffer.BlockCopy(_buffer, (int)bufferPos, buffer, offset + totalRead, toCopy);
                        _position += toCopy;
                        totalRead += toCopy;
                        fromBuffer += toCopy;
                        continue;
                    }
                }

                try
                {
                    _baseStream.Position = _position;
                    int read = _baseStream.Read(buffer, offset + totalRead, count - totalRead);
                    if (read == 0) break;
                    _position += read;
                    totalRead += read;
                    fromStream += read;
                }
                catch
                {
                    Thread.Sleep(20);
                }
            }

            if (fromBuffer + fromStream > 0)
            {
                Debug.WriteLine($"[Read] Req: {count / 1024} KB | Buf: {fromBuffer / 1024} KB | Stream: {fromStream / 1024} KB | Pos: {_position / 1024 / 1024} MB");
            }
            return totalRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            long newPos = origin switch
            {
                SeekOrigin.Begin => offset,
                SeekOrigin.Current => _position + offset,
                SeekOrigin.End => Length + offset,
                _ => throw new NotSupportedException()
            };

            newPos = Math.Clamp(newPos, 0, Length);

            lock (_lock)
            {
                _position = newPos;
                if (newPos < _bufferStart || newPos >= _bufferStart + _bufferValid)
                {
                    _bufferStart = newPos;
                    _bufferValid = 0;
                }
            }

            return newPos;
        }

        public override void Flush() => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _cts.Cancel();
                try { _prefetchTask?.Wait(100); } catch { }
                _cts.Dispose();
                try { _baseStream.Dispose(); } catch { }
            }
            base.Dispose(disposing);
        }
    }
}
