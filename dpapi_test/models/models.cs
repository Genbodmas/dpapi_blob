namespace dpapi_test.models
{
    public sealed class KeyStoreOptions
    {
        public string? DirectoryPath { get; set; }
        public string FileName { get; set; } = "aeskey.blob";
        public int KeySizeBytes { get; set; } = 32; // 32 = AES 256
    }

    public sealed class KeyDto
    {
        public string? KeyBase64 { get; set; }
    }
}
