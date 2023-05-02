using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GQService.com.gq.excel
{
    /// <summary>
    /// 
    /// </summary>
    public class Workbook
    {
        /// <summary>
        /// 
        /// </summary>
        public Workbook()
        {
            this.Worksheets = new List<Worksheet>();
            this.Styles = new List<Style>();
            this.DocumentProperties = new DocumentProperties();
        }

        /// <summary>
        /// 
        /// </summary>
        public List<Worksheet> Worksheets { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public List<Style> Styles { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public DocumentProperties DocumentProperties { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        public void Save(Stream stream)
        {
           XLSXEncode.Encode(this,stream);
        }
    }
}
