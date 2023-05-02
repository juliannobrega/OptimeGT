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
    public class DocumentProperties
    {
        /// <summary>
        /// 
        /// </summary>
        public DocumentProperties()
        {
            this.Author = "";
            this.LastAuthor = "";
            this.Created = DateTime.Now.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        public string Author { get; set;}
        
        /// <summary>
        /// 
        /// </summary>
        public string LastAuthor { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string Created { get; set; }
    }
}
