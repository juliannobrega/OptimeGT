using System;

namespace GQService.com.gq.security
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class SecurityDescription : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public enum SeguridadEstado
        {
            /// <summary>
            /// 
            /// </summary>
            Desactivo,
            /// <summary>
            /// 
            /// </summary>
            Activo,
            /// <summary>
            /// 
            /// </summary>
            SoloLogueo
        }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        //public bool Enabled { get { return Estado != SecurityDescription.SeguridadEstado.Desactivo; `} }
        /// <summary>
        /// 
        /// </summary>
        public SeguridadEstado Estado { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string[] Perfiles { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        public SecurityDescription(string name)
        {
            Name = name;
            Estado = SecurityDescription.SeguridadEstado.Activo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="perfiles"></param>
        public SecurityDescription(string name, string[] perfiles)
        {
            Name = name;
            Perfiles = perfiles;
            Estado = SecurityDescription.SeguridadEstado.Activo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="estado"></param>
        public SecurityDescription(SecurityDescription.SeguridadEstado estado)
        {
            Estado = estado;
        }
    }
}
