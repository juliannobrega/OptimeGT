using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GQService.com.gq.excel
{
    /// <summary>
    /// 
    /// </summary>
    public class Style
    {
        /// <summary>
        /// 
        /// </summary>
        public Style()
        {
            this.Id = "";
            this.Name = "";
            this.Elements = new List<StyleElement>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        public Style(string Id)
        {
            this.Id = Id;
            this.Name = "";
            this.Elements = new List<StyleElement>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Name"></param>
        public Style(string Id, string Name)
        {
            this.Id = Id;
            this.Name = Name;
            this.Elements = new List<StyleElement>();
        }

        /// <summary>
        /// 
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<StyleElement> Elements { get; set; }

    }
}
