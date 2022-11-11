using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp11
{
    public class clsRinjdael
    {
        private static string stringKey = "s@ngh14ng";



        /// <summary>
        /// Decrypt
        /// </summary>
        /// <param name="stringInputText"></param>
        /// <returns></returns>
        public static string Decrypt(string stringInputText)
        {
            RijndaelManaged RijndaelCipher = new RijndaelManaged();
            byte[] EncryptedData = Convert.FromBase64String(stringInputText);
            byte[] Salt = Encoding.ASCII.GetBytes(stringKey.Length.ToString());
            PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(stringKey, Salt);
            ICryptoTransform Decryptor = RijndaelCipher.CreateDecryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));
            MemoryStream memoryStream = new MemoryStream(EncryptedData);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, Decryptor, CryptoStreamMode.Read);
            byte[] PlainText = new byte[EncryptedData.Length];
            int DecryptedCount = cryptoStream.Read(PlainText, 0, PlainText.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.Unicode.GetString(PlainText, 0, DecryptedCount);
        }

        
    }
}
