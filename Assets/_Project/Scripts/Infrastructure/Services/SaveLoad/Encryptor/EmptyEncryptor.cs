public class EmptyEncryptor : IEncryptor
{
    public string Encrypt(string data)
    {
        return data;
    }

    public string Decrypt(string encryptedData)
    {
        return encryptedData;
    }
}