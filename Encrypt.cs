using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;
using System.Windows.Forms;

namespace Boostera
{
    public class Encrypt
    {
        private string boosteraKeyFolder = Program.BoosteraDataFolder;

        public Encrypt(string boosteraKeyFolder)
        {
            this.boosteraKeyFolder = boosteraKeyFolder;
        }

        public void CreateKey()
        {
            if (!Directory.Exists(boosteraKeyFolder)) Directory.CreateDirectory(boosteraKeyFolder);
            if (!File.Exists(Path.Combine(boosteraKeyFolder, "Boostera.key")))
            {
                using (var rsa = new RSACryptoServiceProvider(2048))
                {
                    var privateKey = rsa.ToXmlString(true);
                    File.WriteAllText(Path.Combine(boosteraKeyFolder, "Boostera.key"), privateKey);

                    MessageBox.Show("接続情報保護用のシークレットが作成されました。\nこれは他の人に共有しないように注意してください。\n\n" +
                        Path.Combine(boosteraKeyFolder, "Boostera.key"), "Boostera");
                }
            }
        }

        public HistoryEncrypted EncryptData(string data)
        {
            if (!File.Exists(Path.Combine(boosteraKeyFolder, "Boostera.key"))) CreateKey();

            var key = Encoding.UTF8.GetBytes(Membership.GeneratePassword(32, 0));
            var iv = Encoding.UTF8.GetBytes(Membership.GeneratePassword(16, 0));

            using (var myRijndael = new RijndaelManaged())
            {
                myRijndael.BlockSize = 128;
                myRijndael.KeySize = 256;
                myRijndael.Mode = CipherMode.CBC;
                myRijndael.Padding = PaddingMode.PKCS7;
                myRijndael.Key = key;
                myRijndael.IV = iv;

                var encryptor = myRijndael.CreateEncryptor(myRijndael.Key, myRijndael.IV);

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

                var rsa = new RSACryptoServiceProvider(2048);
                rsa.FromXmlString(File.ReadAllText(Path.Combine(boosteraKeyFolder, "Boostera.key")));

                var historyEncrypted = new HistoryEncrypted(
                    Convert.ToBase64String(rsa.Encrypt(key, false)), Convert.ToBase64String(rsa.Encrypt(iv, false)), Convert.ToBase64String(encrypted));
                return (historyEncrypted);
            }
        }

        public string DecryptData(HistoryEncrypted historyEncrypted)
        {
            var rsa = new RSACryptoServiceProvider(2048);
            rsa.FromXmlString(File.ReadAllText(Path.Combine(boosteraKeyFolder, "Boostera.key")));

            var key = rsa.Decrypt(Convert.FromBase64String(historyEncrypted.Key), false);
            var iv = rsa.Decrypt(Convert.FromBase64String(historyEncrypted.Iv), false);

            using (var rijndael = new RijndaelManaged())
            {
                rijndael.BlockSize = 128;
                rijndael.KeySize = 256;
                rijndael.Mode = CipherMode.CBC;
                rijndael.Padding = PaddingMode.PKCS7;
                rijndael.Key = key;
                rijndael.IV = iv;

                var decryptor = rijndael.CreateDecryptor(rijndael.Key, rijndael.IV);

                var plain = string.Empty;
                using (var mStream = new MemoryStream(Convert.FromBase64String(historyEncrypted.Data)))
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
