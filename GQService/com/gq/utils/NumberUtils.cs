using System;
using System.Globalization;

namespace GQService.com.gq.utils
{
    public static class NumberUtils
    {
        public static T ConvertTo<T>(string value) where T : IConvertible, IComparable
        {
            string separator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            string replace = separator == "." ? "," : ".";
            return (T)Convert.ChangeType(value.Replace(replace, separator), typeof(T));
        }

        public static Double ConvertToDouble(string value)
        {
            try
            {
                NumberFormatInfo nfi = System.Globalization.CultureInfo.CurrentCulture.NumberFormat;
                string SeparadorDecimal = nfi.NumberDecimalSeparator;

                value = value.Replace(".", SeparadorDecimal);
                var valueDouble = Convert.ToDouble(value);
                return valueDouble;
            }
            catch (Exception)
            {
                return -9999;
            }
        }

        public static String[] SplitDouble(string value)
        {   
            NumberFormatInfo nfi = System.Globalization.CultureInfo.CurrentCulture.NumberFormat;
            string SeparadorDecimal = nfi.NumberDecimalSeparator;

            value = value.Replace(".", SeparadorDecimal);
            return value.Split(SeparadorDecimal[0]);
        }
    }
}
