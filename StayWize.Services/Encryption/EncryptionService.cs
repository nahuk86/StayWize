using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace StayWize.Services.Encryption;

public class EncryptionService : IEncryptionService
{
    private readonly byte[] _key;

    public EncryptionService(IConfiguration configuration)
    {
        var secretKey = configuration["EncryptionSettings:SecretKey"]
            ?? throw new InvalidOperationException("EncryptionSettings:SecretKey no está configurado.");

        // Aseguramos exactamente 32 bytes para AES-256
        _key = SHA256.HashData(Encoding.UTF8.GetBytes(secretKey));
    }

    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText)) return plainText;

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        var ivBase64 = Convert.ToBase64String(aes.IV);
        var cipherBase64 = Convert.ToBase64String(cipherBytes);

        return $"{ivBase64}:{cipherBase64}";
    }

    public string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText)) return cipherText;

        var parts = cipherText.Split(':');
        if (parts.Length != 2) return cipherText;

        var iv = Convert.FromBase64String(parts[0]);
        var cipherBytes = Convert.FromBase64String(parts[1]);

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

        return Encoding.UTF8.GetString(plainBytes);
    }
}