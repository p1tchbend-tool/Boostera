using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Boostera
{
    public class Encrypt
    {
        private string boosteraKeyFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        public Encrypt(string boosteraKeyFolder)
        {
            this.boosteraKeyFolder = boosteraKeyFolder;
        }

        private bool CreateKey(string key)
        {
            if (string.IsNullOrEmpty(key)) return false;

            try
            {
                if (!Directory.Exists(boosteraKeyFolder)) Directory.CreateDirectory(boosteraKeyFolder);
                var bytes = Encoding.UTF8.GetBytes(key);
                File.WriteAllText(Path.Combine(boosteraKeyFolder, "Boostera.Key"), Convert.ToBase64String(bytes));

                return true;
            }
            catch { return false; }
        }

        public string EncryptString(string str)
        {
            var bytes = Convert.FromBase64String(File.ReadAllText(Path.Combine(boosteraKeyFolder, "Boostera.Key")));
            var key = Encoding.UTF8.GetString(bytes);

            using (Aes aesAlg = Aes.Create())
            {
                using (var keyDerivationFunction = new Rfc2898DeriveBytes(key, 16))
                {
                    aesAlg.Key = keyDerivationFunction.GetBytes(32);
                    aesAlg.IV = keyDerivationFunction.GetBytes(16);
                }

                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, aesAlg.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(str);
                        }
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        public string DecryptString(string str)
        {
            var bytes = Convert.FromBase64String(File.ReadAllText(Path.Combine(boosteraKeyFolder, "Boostera.Key")));
            var key = Encoding.UTF8.GetString(bytes);

            using (Aes aesAlg = Aes.Create())
            {
                using (var keyDerivationFunction = new Rfc2898DeriveBytes(key, 16))
                {
                    aesAlg.Key = keyDerivationFunction.GetBytes(32);
                    aesAlg.IV = keyDerivationFunction.GetBytes(16);
                }

                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(str)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, aesAlg.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
