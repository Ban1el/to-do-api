using System.Security.Cryptography;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace API.Extensions;

public static class CryptoExtensions
{
    private static byte[]? Key;

    public static void SetKey(string base64Key)
    {
        if (string.IsNullOrWhiteSpace(base64Key))
            throw new ArgumentException("Key cannot be null or empty", nameof(base64Key));

        Key = Convert.FromBase64String(base64Key);

        if (Key.Length != 32)
            throw new InvalidOperationException("Encryption key must be 32 bytes.");
    }

    public static string Encrypt(this string plainText)
    {
        if (Key == null) throw new InvalidOperationException("Invalid Encryption.");

        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

        byte[] iv = RandomNumberGenerator.GetBytes(12);

        byte[] cipherBytes = new byte[plainBytes.Length];
        byte[] tag = new byte[16];

        using (var aesGcm = new AesGcm(Key, tag.Length))
        {
            aesGcm.Encrypt(iv, plainBytes, cipherBytes, tag);
        }

        byte[] result = new byte[iv.Length + tag.Length + cipherBytes.Length];
        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
        Buffer.BlockCopy(tag, 0, result, iv.Length, tag.Length);
        Buffer.BlockCopy(cipherBytes, 0, result, iv.Length + tag.Length, cipherBytes.Length);

        return Convert.ToBase64String(result);
    }

    public static string Decrypt(this string encryptedText)
    {
        if (Key == null) throw new InvalidOperationException("Invalid Decryption.");

        byte[] combined = Convert.FromBase64String(encryptedText);

        byte[] iv = new byte[12];
        byte[] tag = new byte[16];
        byte[] cipherBytes = new byte[combined.Length - iv.Length - tag.Length];

        Buffer.BlockCopy(combined, 0, iv, 0, iv.Length);
        Buffer.BlockCopy(combined, iv.Length, tag, 0, tag.Length);
        Buffer.BlockCopy(combined, iv.Length + tag.Length, cipherBytes, 0, cipherBytes.Length);

        byte[] plainBytes = new byte[cipherBytes.Length];

        using (var aesGcm = new AesGcm(Key, tag.Length))
        {
            aesGcm.Decrypt(iv, cipherBytes, tag, plainBytes);
        }

        return Encoding.UTF8.GetString(plainBytes);
    }
}
