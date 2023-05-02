using GQService.com.gq.dto;
using System;

namespace GQService.com.gq.exception
{
    /// <summary>
    /// 
    /// </summary>
    public class SecurityException : Exception
    {
        private string message = "No tiene permiso para acceder a este metodo";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="Method"></param>
        public SecurityException(object value, string Method)
        {
            ObjectName = security.Security.getNameObject(value);
            MethodName = Method;
        }

        /// <summary>
        /// 
        /// </summary>
        public override string Message { get { return message; } }

        /// <summary>
        /// 
        /// </summary>
        public virtual IGenericDto Usuario { get { return security.Security.usuarioLogueado; } }

        /// <summary>
        /// 
        /// </summary>
        public virtual string ObjectName { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string MethodName { get; private set; }
    }
}
