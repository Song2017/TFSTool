﻿using System;
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
        public static string GetIndexsSelected(this CheckedListBox checkedListBox)
        {
            string indexsSelected = string.Empty;
            for (int i = 0; i < checkedListBox.Items.Count; i++) {
                if (checkedListBox.GetItemChecked(i))
                    indexsSelected += i.ToStringEx() + ",";
            }
             
            return indexsSelected.TrimEnd(',');
        }
        public static void SetIndexsSelected(this CheckedListBox checkedListBox, string indexsSelected, bool isSelected = true)
        {
            foreach (var index in indexsSelected.Split(','))
            {
                checkedListBox.SetItemChecked(Int32.Parse(index), isSelected);
            }
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
            using (Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(CryptoConstants.ENCRYPTIONKEY, saltStringBytes, CryptoConstants.DERIVATIONITERATIONS))
            {
                byte[] keyBytes = password.GetBytes(CryptoConstants.PASSWORDSIZE);
                using (RijndaelManaged symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = CryptoConstants.BLOCKSIZE;
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
            string sourcePass;
            using (Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(CryptoConstants.ENCRYPTIONKEY, saltStringBytes, CryptoConstants.DERIVATIONITERATIONS))
            {
                byte[] keyBytes = password.GetBytes(CryptoConstants.PASSWORDSIZE);
                using (RijndaelManaged symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = CryptoConstants.BLOCKSIZE;
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
                                sourcePass = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
            return sourcePass;
        }

        public static byte[] Generate256BitsOfRandomEntropy()
        {
            byte[] randomBytes = new byte[CryptoConstants.PASSWORDSIZE];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomBytes);
            }
            return randomBytes;
        }

    }
}
