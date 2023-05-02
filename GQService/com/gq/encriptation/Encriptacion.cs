using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace GQService.com.gq.encriptation
{
    /// <summary>
    /// Proveee servicios para encripatacion de strings usando MD5
    /// </summary>
    public static class Encriptacion
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="strKey"></param>
        /// <returns></returns>
        public static string Encriptar(string value, string strKey)
        {
            return encrypt(value, strKey);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="strKey"></param>
        /// <returns></returns>
        public static string Desencriptar(string value, string strKey)
        {
            return decrypt(value, strKey);
        }

        /// <summary>
        /// Encrypt the given string using the specified key.
        /// </summary>
        /// <param name="strToEncrypt">The string to be encrypted.</param>
        /// <param name="strKey">The encryption key.</param>
        /// <returns>The encrypted string.</returns>
        private static string encrypt(string strToEncrypt, string strKey)
        {
            if (string.IsNullOrWhiteSpace(strToEncrypt))
                strToEncrypt = "";
            try
            {
                TripleDES objDESCrypto = TripleDES.Create();
                MD5 objHashMD5 = MD5.Create();
                byte[] byteHash, byteBuff;
                string strTempKey = strKey;
                byteHash = objHashMD5.ComputeHash(ASCIIEncoding.UTF8.GetBytes(strTempKey));
                byteHash = Combine(byteHash, SubArray(byteHash, 0, 8));
                objHashMD5 = null;
                objDESCrypto.Key = byteHash;
                objDESCrypto.Mode = CipherMode.ECB; //CBC, CFB
                byteBuff = ASCIIEncoding.UTF8.GetBytes(strToEncrypt);
                return Convert.ToBase64String(objDESCrypto.CreateEncryptor().
                    TransformFinalBlock(byteBuff, 0, byteBuff.Length));
            }
            catch (Exception ex)
            {
                return "Wrong Input. " + ex.Message;
            }
        }

        public static byte[] Combine(params byte[][] arrays)
        {
            byte[] rv = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;
            foreach (byte[] array in arrays)
            {
                System.Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                offset += array.Length;
            }
            return rv;
        }

        public static byte[] SubArray(byte[] array, int startIndex, int count = 0)
        {
            byte[] subArray = new byte[count];
            System.Buffer.BlockCopy(array, startIndex, subArray, 0, count);
            return subArray;
        }

        /// <summary>
        /// Decrypt the given string using the specified key.
        /// </summary>
        /// <param name="strEncrypted">The string to be decrypted.</param>
        /// <param name="strKey">The decryption key.</param>
        /// <returns>The decrypted string.</returns>
        private static string decrypt(string strEncrypted, string strKey)
        {
            if (!string.IsNullOrWhiteSpace(strEncrypted))
            {
                try
                {
                    TripleDES objDESCrypto = TripleDES.Create();
                    MD5 objHashMD5 = MD5.Create();
                    byte[] byteHash, byteBuff;
                    string strTempKey = strKey;
                    byteHash = objHashMD5.ComputeHash(ASCIIEncoding.UTF8.GetBytes(strTempKey));
                    byteHash = Combine(byteHash, SubArray(byteHash, 0, 8));
                    objHashMD5 = null;
                    objDESCrypto.Key = byteHash;
                    objDESCrypto.Mode = CipherMode.ECB; //CBC, CFB
                    byteBuff = Convert.FromBase64String(strEncrypted);
                    string strDecrypted = ASCIIEncoding.UTF8.GetString
                    (objDESCrypto.CreateDecryptor().TransformFinalBlock
                    (byteBuff, 0, byteBuff.Length));
                    objDESCrypto = null;
                    return strDecrypted;
                }
                catch (Exception ex)
                {
                    return "Wrong Input. " + ex.Message;
                }
            }

            return strEncrypted;
        }
    }
}