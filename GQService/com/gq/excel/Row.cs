using System.Collections.Generic;

namespace GQService.com.gq.excel
{
    /// <summary>
    /// 
    /// </summary>
    public class Row
    {
        /// <summary>
        /// 
        /// </summary>
        public Row()
        {
            Cells = new Dictionary<int, Cell>();
            StyleID = "Default";
        }

        /// <summary>
        /// /
        /// </summary>
        public string StyleID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<int, Cell> Cells { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="col"></param>
        /// <param name="cell"></param>
        /// <returns></returns>
        public Cell AddCell(int col, Cell cell)
        {
            if (Cells.ContainsKey(col))
            {
                Cells.Remove(col);
            }
            Cells.Add(col, cell);
            return cell;
        }
    }
}
