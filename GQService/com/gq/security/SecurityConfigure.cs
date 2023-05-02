using GQService.com.gq.dto;
using System;
using System.Collections.Generic;

namespace GQService.com.gq.security
{
    /// <summary>
    /// 
    /// </summary>
    public class SecurityConfigure
    {
        /// <summary>
        /// 
        /// </summary>
        public static void Configure(
            TimeSpan timeOut,
            Security.DelegateHasPermission delegateHasPermission = null,
            Security.DelegateCheckUsuarioLoginKey delegateCheckUsuarioLoginKey = null,
            Type[] excludeControllerSecurity = null//, Boolean useContainerSession = false
            )
        {

            SecurityConfigure sc = new SecurityConfigure();
            sc.UseContainerSession = true; // useContainerSession;
            sc.TimeOut = timeOut;

            structureMap.ObjectFactory.Configure(p =>
            {
                p.For<SecurityConfigure>().Singleton().Use(sc).Named("SecurityConfigure");
                p.For<Security.DelegateHasPermission>().Singleton().Use(delegateHasPermission).Named("delegateHasPermission");
                p.For<Security.DelegateCheckUsuarioLoginKey>().Singleton().Use(delegateCheckUsuarioLoginKey).Named("delegateCheckUsuarioLoginKey");
                p.For<Type[]>().Singleton().Use(excludeControllerSecurity).Named("excludeControllerSecurity");
                p.For<Dictionary<String, SessionUser>>().Singleton().Use<Dictionary<String, SessionUser>>(() => new Dictionary<String, SessionUser>()).Named("mobileSession");
            });
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual Boolean UseContainerSession { get; private set; }

        public virtual TimeSpan TimeOut { get; private set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class SessionUser
    {
        /// <summary>
        /// 
        /// </summary>
        private IGenericDto _User = null;

        /// <summary>
        /// 
        /// </summary>
        public virtual IGenericDto User
        {
            get { Ticks = DateTime.Now.Ticks; return _User; }
            set
            {
                _User = value;
                isNew = false;
                Ticks = DateTime.Now.Ticks;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual string SessionId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual long Ticks { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool isNew { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public SessionUser()
        {
            Ticks = DateTime.Now.Ticks;
            isNew = true;
        }
    }
}
