namespace SmartExpenseTracker.UI.Services
{
    public class ProgressStream : Stream
    {
        private readonly Stream _inner;
        private readonly Action<int> _progress;
        private long _totalRead = 0;

        public ProgressStream(Stream inner, Action<int> progress)
        {
            _inner = inner;
            _progress = progress;
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            var read = await _inner.ReadAsync(buffer, offset, count, cancellationToken);

            if (read > 0)
            {
                _totalRead += read;
                var percent = (int)((double)_totalRead / _inner.Length * 100);
                _progress(percent);
            }

            return read;
        }

        public override bool CanRead => _inner.CanRead;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => _inner.Length;
        public override long Position { get => _inner.Position; set => throw new NotSupportedException(); }
        public override void Flush() => _inner.Flush();
        public override int Read(byte[] buffer, int offset, int count) => _inner.Read(buffer, offset, count);
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }

}
