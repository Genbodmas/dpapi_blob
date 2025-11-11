namespace dpapi_test.models
{
    public interface IKeyStore
    {
        Task<byte[]> GetOrCreateKeyAsync(CancellationToken ct = default);
        Task<byte[]> UnprotectAsync(CancellationToken ct = default);
        Task ProtectAndSaveAsync(byte[] rawKey, CancellationToken ct = default);
    }
}
