using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Web.Security;

namespace Boostera
{
    public class Encrypt
    {
        private string boosteraKeyPath = Path.Combine(Application.StartupPath, "Boostera.key");

        public Encrypt(string boosteraKeyPath)
        {
            this.boosteraKeyPath = boosteraKeyPath;
            CreateKey();
        }

        private void CreateKey()
        {
            if (!File.Exists(boosteraKeyPath))
            {
                var key = Membership.GeneratePassword(32, 0);
                var iv = Membership.GeneratePassword(16, 0);

                var dirPath = Path.GetDirectoryName(boosteraKeyPath);
                if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
                File.WriteAllText(boosteraKeyPath, key + iv);
            }
        }

        public string EncryptString(string str)
        {
            var text = File.ReadAllText(boosteraKeyPath);
            var key = text.Substring(0, 32);
            var iv = text.Substring(32);

            using (var myRijndael = new RijndaelManaged())
            {
                myRijndael.BlockSize = 128;
                myRijndael.KeySize = 256;
                myRijndael.Mode = CipherMode.CBC;
                myRijndael.Padding = PaddingMode.PKCS7;
                myRijndael.Key = Encoding.UTF8.GetBytes(key);
                myRijndael.IV = Encoding.UTF8.GetBytes(iv);

                var encryptor = myRijndael.CreateEncryptor(myRijndael.Key, myRijndael.IV);

                byte[] encrypted;
                using (var mStream = new MemoryStream())
                {
                    using (var ctStream = new CryptoStream(mStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (var sw = new StreamWriter(ctStream))
                        {
                            sw.Write(str);
                        }
                        encrypted = mStream.ToArray();
                    }
                }
                return (Convert.ToBase64String(encrypted));
            }
        }

        public string DecryptString(string str)
        {
            var text = File.ReadAllText(boosteraKeyPath);
            var key = text.Substring(0, 32);
            var iv = text.Substring(32);

            using (var rijndael = new RijndaelManaged())
            {
                rijndael.BlockSize = 128;
                rijndael.KeySize = 256;
                rijndael.Mode = CipherMode.CBC;
                rijndael.Padding = PaddingMode.PKCS7;
                rijndael.Key = Encoding.UTF8.GetBytes(key);
                rijndael.IV = Encoding.UTF8.GetBytes(iv);
                
                var decryptor = rijndael.CreateDecryptor(rijndael.Key, rijndael.IV);

                var plain = string.Empty;
                using (var mStream = new MemoryStream(Convert.FromBase64String(str)))
                {
                    using (var ctStream = new CryptoStream(mStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (var sr = new StreamReader(ctStream))
                        {
                            plain = sr.ReadLine();
                        }
                    }
                }
                return plain;
            }
        }
    }
}
