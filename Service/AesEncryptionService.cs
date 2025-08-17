using System.Security.Cryptography;
using System.Text;

namespace SafeVault.Service;

public class AesEncryptionService
{
    public byte[] iv = new byte[16];
    private readonly DynamoDbService _dynamoDbService;

    public AesEncryptionService(DynamoDbService dynamoDbService)
    {
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(iv);
        }

        _dynamoDbService = dynamoDbService;
    }
    
    public byte[] EncryptString(string plainText, byte[] iv)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(_dynamoDbService.GetEncryptionKey());
        byte[] cipheredtext;
    
        using (Aes aes = Aes.Create())
        {
            ICryptoTransform encryptor = aes.CreateEncryptor(keyBytes, iv);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                    {
                        streamWriter.Write(plainText);
                    }

                    cipheredtext = memoryStream.ToArray();
                }
            }
        }
        return cipheredtext;
    }
    
    public string DecryptString(byte[] cipheredText, byte[] iv)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(_dynamoDbService.GetEncryptionKey());
        string plainText = String.Empty;
        
        using (Aes aes = Aes.Create())
        {
            ICryptoTransform decryptor = aes.CreateDecryptor(keyBytes, iv);
            using (MemoryStream memoryStream = new MemoryStream(cipheredText))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader streamReader = new StreamReader(cryptoStream))
                    {
                        plainText = streamReader.ReadToEnd();
                    }
                }
            }
        }
        return plainText;
    }
}