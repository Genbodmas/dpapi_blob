using dpapi_test.models;
using Microsoft.Extensions.Options;

namespace dpapi_test.Utils
{
    public sealed class DpapiKeyStore : IKeyStore
    {
        private readonly KeyStoreOptions _opt;
        private readonly string _blobPath;

        public DpapiKeyStore(IOptions<KeyStoreOptions> opt)
        {
            _opt = opt.Value;
            if (string.IsNullOrWhiteSpace(_opt.DirectoryPath))
                throw new InvalidOperationException("KeyStore.directoryPath is required");

            Directory.CreateDirectory(_opt.DirectoryPath);
            _blobPath = Path.Combine(_opt.DirectoryPath, _opt.FileName);
        }

        public async Task<byte[]> GetOrCreateKeyAsync(CancellationToken ct = default)
        {
            if (File.Exists(_blobPath))
                return await UnprotectAsync(ct);

            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            var raw = new byte[_opt.KeySizeBytes];
            rng.GetBytes(raw);

            await ProtectAndSaveAsync(raw, ct);

            var copy = new byte[raw.Length];
            Buffer.BlockCopy(raw, 0, copy, 0, raw.Length);
            Array.Clear(raw, 0, raw.Length);
            return copy;
        }

        public async Task ProtectAndSaveAsync(byte[] rawKey, CancellationToken ct = default)
        {
            var protectedBlob = System.Security.Cryptography.ProtectedData.Protect(rawKey,optionalEntropy: null,scope: System.Security.Cryptography.DataProtectionScope.LocalMachine);

            var tmp = _blobPath + ".tmp";
            await File.WriteAllBytesAsync(tmp, protectedBlob, ct);
            File.Move(tmp, _blobPath, overwrite: true);

            Array.Clear(protectedBlob, 0, protectedBlob.Length);
        }

        public async Task<byte[]> UnprotectAsync(CancellationToken ct = default)
        {
            if (!File.Exists(_blobPath))
            {
                throw new FileNotFoundException("blob not found", _blobPath);
            }
               

            var blob = await File.ReadAllBytesAsync(_blobPath, ct);
            try
            {
                var raw = System.Security.Cryptography.ProtectedData.Unprotect(blob,optionalEntropy: null,scope: System.Security.Cryptography.DataProtectionScope.LocalMachine);

                return raw;
            }
            finally
            {
                Array.Clear(blob, 0, blob.Length);
            }
        }
    }
}
