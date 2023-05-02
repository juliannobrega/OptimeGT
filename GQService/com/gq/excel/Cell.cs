using System;

namespace GQService.com.gq.excel
{
    /// <summary>
    /// 
    /// </summary>
    public class Cell
    {
        /// <summary>
        /// 
        /// </summary>
        public enum CellType
        {
            /// <summary>
            /// 
            /// </summary>
            String,
            /// <summary>
            /// 
            /// </summary>
            Number,
            /// <summary>
            /// 
            /// </summary>
            Function
        }

        /// <summary>
        /// 
        /// </summary>
        public Cell()
        {
            Data = "";
            Type = CellType.String;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public Cell(object data)
        {
            Data = data.ToString().Replace("\0", "");
            Type = CellType.String;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="type"></param>
        public Cell(object data, CellType type)
        {
            Data = data.ToString().Replace("\0", "");
            Type = type;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Formula { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CellType Type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnNumber"></param>
        /// <returns></returns>
        public static string GetExcelColumnName(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static int GetExcelColumnIndex(string columnName)
        {
            if (string.IsNullOrEmpty(columnName)) throw new ArgumentNullException("columnName");

            columnName = columnName.ToUpperInvariant();

            int sum = 0;

            for (int i = 0; i < columnName.Length; i++)
            {
                sum *= 26;
                sum += (columnName[i] - 'A' + 1);
            }

            return sum;
        }
    }
}
