using System;

namespace GQService.com.gq.data
{
    /// <summary>
    /// 
    /// </summary>
    public class ReturnData
    {
        /// <summary>
        /// 
        /// </summary>
        public object data { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool isError { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long ticks { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool isSecurity { get;  set; }

        /// <summary>
        /// 
        /// </summary>
        public ReturnData()
        {
            data = null;
            isError = false;
            isSecurity = false;
            ticks = DateTime.Now.Ticks;
        }

    }
}