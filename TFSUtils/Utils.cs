using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace TFSUtils
{
    public static class Utils
    {
        //string
        public static bool IsNullOrEmpty(this string str)
        {
            return str == null | str == string.Empty;
        }

        public static string ToStringEx(this object str)
        {
            if (str == null)
            {
                return string.Empty;
            }
            return str.ToString();
        }


        //CheckedListBox
        public static bool AllSelected(this CheckedListBox checkedListBox1, bool toSelect)
        {
            if (checkedListBox1 == null)
                return false;
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, toSelect);
            }

            return true;
        }

        public static bool HasSelected(this CheckedListBox checkedListBox, out string valueSelected)
        {
            bool hasSelected = false;

            valueSelected = string.Empty;
            foreach (var item in checkedListBox.CheckedItems)
                valueSelected += item.ToStringEx() + ",";

            if (!valueSelected.IsNullOrEmpty())
                hasSelected = true;

            return hasSelected;
        }


        //app.config
        public static void SaveConfig(string key, string appValue)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            AppSettingsSection ass = config.AppSettings;
            for (int i = 0; i < ass.Settings.AllKeys.Length; i++)
            {
                if (key.ToStringEx().Equals(ass.Settings.AllKeys[i]))
                {
                    ass.Settings.Remove(key);
                }
            }
            ass.Settings.Add(key, Utils.Encrypt(appValue));
            config.Save(ConfigurationSaveMode.Modified);
        }

        public static string GetConfig(string key, string defaultValue = "", bool isDecrypt=true)
        {
            string strConfigRtn = string.Empty;
            AppSettingsSection ass = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).AppSettings;
            if (ass == null || ass.Settings == null || ass.Settings.Count == 0 || ass.Settings[key] == null || ass.Settings[key].Value == null)
            {
                Utils.SaveConfig(key, defaultValue);
                return defaultValue;
            }
            strConfigRtn = ass.Settings[key].Value.ToString();
            if (isDecrypt)
                return Utils.Decrypt(strConfigRtn);
            else
                return strConfigRtn;
        }

        public static string Encrypt(string source)
        {
            byte[] saltStringBytes = Utils.Generate256BitsOfRandomEntropy();
            byte[] ivStringBytes = Utils.Generate256BitsOfRandomEntropy();
            byte[] sourceBytes = Encoding.UTF8.GetBytes(source);
            string result;
            using (Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(ENCRYPTIONKEY, saltStringBytes, 1000))
            {
                byte[] keyBytes = password.GetBytes(32);
                using (RijndaelManaged symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(sourceBytes, 0, sourceBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                result = Convert.ToBase64String(saltStringBytes.Concat(ivStringBytes).ToArray<byte>().Concat(memoryStream.ToArray()).ToArray<byte>());
                            }
                        }
                    }
                }
            }
            return result;
        }

        public static string Decrypt(string cipherText)
        {
            byte[] cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
            byte[] saltStringBytes = cipherTextBytesWithSaltAndIv.Take(32).ToArray<byte>();
            byte[] ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(32).Take(32).ToArray<byte>();
            byte[] cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip(64).Take(cipherTextBytesWithSaltAndIv.Length - 64).ToArray<byte>();
            string @string;
            using (Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(ENCRYPTIONKEY, saltStringBytes, 1000))
            {
                byte[] keyBytes = password.GetBytes(32);
                using (RijndaelManaged symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (MemoryStream memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                byte[] plainTextBytes = new byte[cipherTextBytes.Length];
                                int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                @string = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
            return @string;
        }

        public static byte[] Generate256BitsOfRandomEntropy()
        {
            byte[] randomBytes = new byte[32];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomBytes);
            }
            return randomBytes;
        }

        private const int KEYSIZE = 256;

        private const int DERIVATIONITERATIONS = 1000;

        private const string ENCRYPTIONKEY = "ENCRYPTIONKEY";
    }
}
