namespace GQService.com.gq.excel
{
    /// <summary>
    /// 
    /// </summary>
    public class StyleAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        public StyleAttribute()
        {
            Name = "";
            Value = "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        public StyleAttribute(string Name, string Value)
        {
            this.Name = Name;
            this.Value = Value;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Value { get; set; }
    }
}
