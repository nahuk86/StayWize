using FluentAssertions;
using Microsoft.Extensions.Configuration;
using StayWize.Services.Encryption;

namespace StayWize.UnitTests.Services;

public class EncryptionServiceTests
{
    private readonly IEncryptionService _encryptionService;

    public EncryptionServiceTests()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["EncryptionSettings:SecretKey"] = "test-encryption-key-32-bytes-long!"
            })
            .Build();

        _encryptionService = new EncryptionService(config);
    }

    [Fact]
    public void Encrypt_ThenDecrypt_ShouldReturnOriginalValue()
    {
        var original = "35123456";

        var encrypted = _encryptionService.Encrypt(original);
        var decrypted = _encryptionService.Decrypt(encrypted);

        decrypted.Should().Be(original);
    }

    [Fact]
    public void Encrypt_SameValueTwice_ShouldProduceDifferentResults()
    {
        var value = "35123456";

        var encrypted1 = _encryptionService.Encrypt(value);
        var encrypted2 = _encryptionService.Encrypt(value);

        encrypted1.Should().NotBe(encrypted2);
    }

    [Fact]
    public void Encrypt_EmptyString_ShouldReturnEmptyString()
    {
        var result = _encryptionService.Encrypt(string.Empty);

        result.Should().BeEmpty();
    }

    [Fact]
    public void Decrypt_EmptyString_ShouldReturnEmptyString()
    {
        var result = _encryptionService.Decrypt(string.Empty);

        result.Should().BeEmpty();
    }

    [Fact]
    public void Encrypt_ShouldProduceFormatWithColon()
    {
        var encrypted = _encryptionService.Encrypt("test");

        encrypted.Should().Contain(":");
    }
}