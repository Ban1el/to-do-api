using System.Security.Cryptography;
using System.Text;

namespace API.Utilities;

public class CryptoUtils
{
    public string GenerateSalt(int size = 16)
    {
        var saltBytes = RandomNumberGenerator.GetBytes(size);
        return Convert.ToBase64String(saltBytes);
    }

    public string HashPassword(string password, string salt)
    {
        byte[] saltBytes = Convert.FromBase64String(salt);

        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password: Encoding.UTF8.GetBytes(password),
            salt: saltBytes,
            iterations: 100_000,
            hashAlgorithm: HashAlgorithmName.SHA512,
            outputLength: 64
        );

        return Convert.ToHexString(hash);
    }

    public string GenerateRandomKey()
    {
        byte[] key = RandomNumberGenerator.GetBytes(32);
        string result = Convert.ToBase64String(key);
        return result;
    }
}
