using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GQService.com.gq.utils
{
    public static class UtilsFunctions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="MethodName"></param>
        /// <returns></returns>
        public static string CreateRandomCode(int PasswordLength)
        {
            string _allowedChars = "abcdefghijkmnpqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            Byte[] randomBytes = new Byte[PasswordLength];
            char[] chars = new char[PasswordLength];
            int allowedCharCount = _allowedChars.Length;

            for (int i = 0; i < PasswordLength; i++)
            {
                Random randomObj = new Random();
                randomObj.NextBytes(randomBytes);
                chars[i] = _allowedChars[(int)randomBytes[i] % allowedCharCount];
            }

            return new string(chars);
        }

        public static string FormatearCuit(string value)
        {
            if (value.Length.Equals(11))
            {
                return string.Format("{0} - {1} - {2}", value.Substring(0, 2), value.Substring(2, 8), value.Substring(10, 1));
            }
            else
            {
                return value;
            }
        }
    }
}
