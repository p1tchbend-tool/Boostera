using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Boostera
{
    public class HistoryEncrypted
    {
        public string Key { get; set; }
        public string Iv { get; set; }
        public string Data { get; set; }

        public HistoryEncrypted(string key, string iv, string data)
        {
            Key = key;
            Iv = iv;
            Data = data;
        }

        public static bool CreateKey(string boosteraKeyPath)
        {
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(boosteraKeyPath))) Directory.CreateDirectory(Path.GetDirectoryName(boosteraKeyPath));
                if (!File.Exists(boosteraKeyPath))
                {
                    using (var rsa = new RSACryptoServiceProvider(3072))
                    {
                        var privateKey = rsa.ToXmlString(true);
                        File.WriteAllText(boosteraKeyPath, privateKey);

                        return true;
                    }
                }
                return false;
            }
            catch { throw; }
        }

        public static HistoryEncrypted EncryptData(string data, string boosteraKeyPath)
        {
            if (!File.Exists(boosteraKeyPath)) CreateKey(boosteraKeyPath);

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
                rsa.FromXmlString(File.ReadAllText(boosteraKeyPath));

                var historyEncrypted = new HistoryEncrypted(
                    Convert.ToBase64String(rsa.Encrypt(key, false)), Convert.ToBase64String(rsa.Encrypt(iv, false)), Convert.ToBase64String(encrypted));
                return historyEncrypted;
            }
        }

        public static string DecryptData(HistoryEncrypted historyEncrypted, string boosteraKeyPath)
        {
            var rsa = new RSACryptoServiceProvider(3072);
            rsa.FromXmlString(File.ReadAllText(boosteraKeyPath));

            var key = rsa.Decrypt(Convert.FromBase64String(historyEncrypted.Key), false);
            var iv = rsa.Decrypt(Convert.FromBase64String(historyEncrypted.Iv), false);

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
                using (var mStream = new MemoryStream(Convert.FromBase64String(historyEncrypted.Data)))
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
