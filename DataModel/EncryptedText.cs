using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Boostera
{
    public class EncryptedText
    {
        public string Key { get; set; }
        public string Iv { get; set; }
        public string Data { get; set; }

        public EncryptedText(string key, string iv, string data)
        {
            Key = key;
            Iv = iv;
            Data = data;
        }

        public static bool CreateKey(string keyPath)
        {
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(keyPath))) Directory.CreateDirectory(Path.GetDirectoryName(keyPath));
                if (!File.Exists(keyPath))
                {
                    using (var rsa = new RSACryptoServiceProvider(3072))
                    {
                        var privateKey = rsa.ToXmlString(true);
                        File.WriteAllText(keyPath, privateKey);

                        return true;
                    }
                }
                return false;
            }
            catch { throw; }
        }

        public static EncryptedText Encrypt(string data, string keyPath)
        {
            if (!File.Exists(keyPath)) CreateKey(keyPath);

            var key = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("N"));
            var iv = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("N").Substring(0, 16));

            using (var aes = Aes.Create())
            {
                aes.BlockSize = 128;
                aes.KeySize = 256;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = key;
                aes.IV = iv;

                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                byte[] encrypted;
                using (var mStream = new MemoryStream())
                {
                    using (var ctStream = new CryptoStream(mStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (var sw = new StreamWriter(ctStream))
                        {
                            sw.Write(data);
                        }
                        encrypted = mStream.ToArray();
                    }
                }

                var rsa = new RSACryptoServiceProvider(3072);
                rsa.FromXmlString(File.ReadAllText(keyPath));

                var encryptedText = new EncryptedText(
                    Convert.ToBase64String(rsa.Encrypt(key, false)), Convert.ToBase64String(rsa.Encrypt(iv, false)), Convert.ToBase64String(encrypted));
                return encryptedText;
            }
        }

        public static string Decrypt(EncryptedText encryptedText, string keyPath)
        {
            var rsa = new RSACryptoServiceProvider(3072);
            rsa.FromXmlString(File.ReadAllText(keyPath));

            var key = rsa.Decrypt(Convert.FromBase64String(encryptedText.Key), false);
            var iv = rsa.Decrypt(Convert.FromBase64String(encryptedText.Iv), false);

            using (var aes = Aes.Create())
            {
                aes.BlockSize = 128;
                aes.KeySize = 256;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = key;
                aes.IV = iv;

                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                var plain = string.Empty;
                using (var mStream = new MemoryStream(Convert.FromBase64String(encryptedText.Data)))
                {
                    using (var ctStream = new CryptoStream(mStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (var sr = new StreamReader(ctStream))
                        {
                            plain = sr.ReadToEnd();
                        }
                    }
                }
                return plain;
            }
        }
    }
}
