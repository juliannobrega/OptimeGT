using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GQService.com.gq.menu
{
    /// <summary>
    /// 
    /// </summary>
    public class MenuDescription : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public virtual string Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string Description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string IdParent { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="description"></param>
        /// <param name="idParent"></param>
        public MenuDescription(string id, string description, string idParent)
        {
            Id = id;
            Description = description;
            IdParent = idParent;
        }
    }
}
