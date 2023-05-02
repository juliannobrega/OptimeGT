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
    public class StyleElement
    {
        /// <summary>
        /// 
        /// </summary>
        public StyleElement()
        {
            this.Name = "";
            this.Attribute = new List<StyleAttribute>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        public StyleElement(string Name)
        {
            this.Name = Name;
            this.Attribute = new List<StyleAttribute>();
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<StyleAttribute> Attribute { get; set;}
    }
}
