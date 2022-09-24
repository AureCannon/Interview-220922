namespace Ct.Domain.Models.Streams
{
    public sealed class TemporaryFileStream : IDisposable
    {
        private readonly string _tempFilePath;
        private bool _disposed = false;

        private TemporaryFileStream(string tempFilePath)
        {
            _tempFilePath = tempFilePath;
        }

        public bool TryCreateStreamReader(out Stream? stream)
        {
            if (_disposed)
                throw new InvalidOperationException("Can't create file because it is already disposed.");

            if (string.IsNullOrEmpty(_tempFilePath))
            {
                stream = null;
                return false;
            }

            stream = new FileStream(_tempFilePath, FileMode.Open);
            return true;
        }

        public static async Task<TemporaryFileStream> CreateFromStreamAsync(Stream stream)
        {
            var tempFilePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            var fileStream = new FileStream(tempFilePath, FileMode.Create);

            await using (fileStream)
            {
                await stream.CopyToAsync(fileStream);
            }

            return new TemporaryFileStream(tempFilePath);
        }

        public void Dispose()
        {
            if (_disposed) return;

            File.Delete(_tempFilePath);

            _disposed = true;
        }
    }
}
