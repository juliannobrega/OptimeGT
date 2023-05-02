using GQService.com.gq.dto;
using GQService.com.gq.structureMap;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace GQService.com.gq.security
{
    /// <summary>
    /// 
    /// </summary>
    public class Security
    {
        private static SecurityConfigure sc = null;
        public static SecurityConfigure GetSecurityConfigure
        {
            get
            {
                if (sc == null)
                    sc = ObjectFactory.GetNamedInstance<SecurityConfigure>("SecurityConfigure");
                return sc;
            }
        }

        private static Dictionary<String, SessionUser> sm = null;

        private static object GetMobileSessionLock = new object();
        public static Dictionary<String, SessionUser> GetMobileSession
        {
            get
            {
                if (sm == null)
                    sm = ObjectFactory.GetNamedInstance<Dictionary<String, SessionUser>>("mobileSession");
                return sm;
            }
        }

        private static Dictionary<long, string> UsuariosLoginKey = new Dictionary<long, string>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="LoginKey"></param>
        public static void AddUsuarioLogin(long key ,string LoginKey)
        {
            if (UsuariosLoginKey.ContainsKey(key))
            {
                UsuariosLoginKey[key] = LoginKey;
            }
            else
            {
                UsuariosLoginKey.Add(key, LoginKey);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="loginKey"></param>
        /// <returns></returns>
        public static bool CheckUsuarioLoginKey(long value, string loginKey)
        {
            return UsuariosLoginKey.ContainsKey(value) && UsuariosLoginKey[value].Equals(loginKey);
        }

        public delegate bool DelegateCheckUsuarioLoginKey();

        private static DelegateCheckUsuarioLoginKey delegateCheckUsuarioLoginKey = null;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool CheckUsuarioLoginKey()
        {
            if (delegateCheckUsuarioLoginKey == null)
                delegateCheckUsuarioLoginKey = ObjectFactory.GetNamedInstance<DelegateCheckUsuarioLoginKey>("delegateCheckUsuarioLoginKey");
            if (delegateCheckUsuarioLoginKey != null)
                return delegateCheckUsuarioLoginKey();
            return true;
        }

        //private static SessionUser sessionUser = null;
        /// <summary>
        /// 
        /// </summary>
        public static IGenericDto usuarioLogueado
        {
            get
            {
                SessionUser sessionUser = new SessionUser();

                System.Web.HttpContext.Current.Session.SetString("SessionUser", "");

                lock (GetMobileSessionLock)
                {
                    removeMobileSession();
                    if (GetMobileSession.ContainsKey(System.Web.HttpContext.Current.Session.Id))
                    {
                        SessionUser valueMobile = GetMobileSession[System.Web.HttpContext.Current.Session.Id];
                        if (valueMobile.Ticks + GetSecurityConfigure.TimeOut.Ticks < DateTime.Now.Ticks)
                        {
                            GetMobileSession.Remove(System.Web.HttpContext.Current.Session.Id);
                            sessionUser = new SessionUser();
                        }
                        else
                        {
                            sessionUser = valueMobile;
                        }
                    }
                }

                return sessionUser.User;
            }
            set
            {
                var sessionUser = new SessionUser();
                sessionUser.User = value;
                sessionUser.SessionId = System.Web.HttpContext.Current.Session.Id;

                if (GetSecurityConfigure.UseContainerSession)
                {
                    lock (GetMobileSessionLock)
                    {
                        if (GetMobileSession.ContainsKey(sessionUser.SessionId))
                            GetMobileSession[sessionUser.SessionId] = sessionUser;
                        else
                            GetMobileSession.Add(sessionUser.SessionId, sessionUser);

                        removeMobileSession();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public static IGenericDto GetUserBySession(string sessionId)
        {
            Dictionary<String, SessionUser> mobileSession = ObjectFactory.GetNamedInstance<Dictionary<String, SessionUser>>("mobileSession");
            if (mobileSession.ContainsKey(sessionId))
            {
                SessionUser valueMobile = mobileSession[sessionId];
                if (new DateTime(valueMobile.Ticks).AddDays(1).Ticks < DateTime.Now.Ticks)
                {
                    //FormsAuthentication.SignOut();
                    return null;
                }
                else
                {
                    return valueMobile.User;
                }
            }
            else
            {
                return null;
            }
        }

        

        /// <summary>
        /// 
        /// </summary>
        public static void removeMobileSession()
        {
            try
            {
                long timeLimit = DateTime.Now.Ticks;
                foreach (SessionUser item in GetMobileSession.Values)
                {
                    if (item.Ticks + GetSecurityConfigure.TimeOut.Ticks < timeLimit)
                    {
                        GetMobileSession.Remove(item.SessionId);
                    }
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string getNameObject(object value)
        {
            if (value != null)
            {
                if (value is Type)
                    return (value as Type).Namespace + "." + (value as Type).Name;
                return value.GetType().Namespace + "." + value.GetType().Name;
            }
            return Guid.NewGuid().ToString();
        }

        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="method"></param>
        /// <param name="returnException"></param>
        /// <returns></returns>
        public delegate bool DelegateHasPermission(object value, string method = null, bool returnException = true);

        private static DelegateHasPermission delegateHasPermission = null;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="method"></param>
        /// <param name="returnException"></param>
        /// <returns></returns>
        public static bool hasPermission(object value, string method = null, bool returnException = true)
        {
            if (delegateHasPermission == null)
                delegateHasPermission = ObjectFactory.GetNamedInstance<DelegateHasPermission>("delegateHasPermission");
            if (delegateHasPermission != null)
                return delegateHasPermission(value, method, returnException);
            return true;
        }

        private static Type[] excludeControllerSecurity = null;
        /// <summary>
        /// 
        /// </summary>
        public static Type[] ExcludeControllerSecurity
        {
            get
            {
                if (excludeControllerSecurity == null)
                {
                    excludeControllerSecurity = ObjectFactory.GetNamedInstance<Type[]>("excludeControllerSecurity");
                }
                return excludeControllerSecurity;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsExcludeController(Type type)
        {
            return IsExcludeController(Security.getNameObject(type));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsExcludeController(string name)
        {
            foreach (Type item in ExcludeControllerSecurity)
            {
                if (name.Equals(Security.getNameObject(item)))
                    return true;
            }
            return false;
        }
    }
}