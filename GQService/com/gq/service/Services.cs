using GQService.com.gq.orm;
using GQService.com.gq.structureMap;
using GQService.com.gq.utils;
using FluentNHibernate.Mapping;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace GQService.com.gq.service
{

    /// <summary>
    /// 
    /// </summary>
    public static class Services
    {
        /// <summary>
        /// 
        /// </summary>
        public static ISession session
        {
            get
            {
                return ObjectFactory.GetInstance<ISession>();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static IStatelessSession statelessSession
        {
            get
            {
                return ObjectFactory.GetInstance<IStatelessSession>();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Get<T>() where T : BaseService
        {
            T oService = null;
            int create = 0;
            var constructors = typeof(T).GetConstructors();
            foreach (var item in constructors)
            {
                if (item.GetParameters().Length == 2)
                {
                    create = 1 + create;
                }
                else if (item.GetParameters().Length == 1)
                {
                    if (item.GetParameters()[0].ParameterType == typeof(ISession))
                        create = 2 + create;
                    else if (item.GetParameters()[0].ParameterType == typeof(IStatelessSession))
                        create = 4 + create;
                }
            }

            if (session != null && statelessSession != null && ((create & 1) == 1))
                oService = (T)Activator.CreateInstance(typeof(T), new object[] { session, statelessSession });
            else if (session != null && ((create & 2) == 2))
                oService = (T)Activator.CreateInstance(typeof(T), new object[] { session });
            else if (session != null && ((create & 4) == 4))
                oService = (T)Activator.CreateInstance(typeof(T), new object[] { statelessSession });
            else
                throw new Exception("No tiene session");

            return oService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="session"></param>
        /// <returns></returns>
        public static T Get<T>(ISession session) where T : BaseService
        {
            T oService = (T)Activator.CreateInstance(typeof(T), new object[] { session });
            return oService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="statelessSession"></param>
        /// <returns></returns>
        public static T Get<T>(IStatelessSession statelessSession) where T : BaseService
        {
            T oService = (T)Activator.CreateInstance(typeof(T), new object[] { statelessSession });
            return oService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetNewSession<T>() where T : BaseService
        {
            ISession newSession = GetSessionFactory().OpenSession();
            T oService = (T)Activator.CreateInstance(typeof(T), new object[] { newSession });
            return oService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static ISessionFactory GetSessionFactory(Assembly assembly = null)
        {
            List<Type> typesMap = new List<Type>();
            Type[] types = new Type[] { };

            if (assembly != null)
                types = ClassUtils.getTypes(assembly, typeof(ClassMap<>));
            else
                types = ClassUtils.getTypes(typeof(ClassMap<>));

            while (types.Length == 0)
            {
                Thread.Sleep(1000);
                if (assembly != null)
                    types = ClassUtils.getTypes(assembly, typeof(ClassMap<>));
                else
                    types = ClassUtils.getTypes(typeof(ClassMap<>));
            }


            foreach (Type item in types)
            {
                var typeFound = ClassUtils.getTypes(item.Assembly, item);

                if (typeFound.Length == 0)
                    typesMap.Add(item);
                else
                    typesMap.AddRange(ClassUtils.getTypes(item.Assembly, item));
            }

            SessionHelper.BuildSessionFactory(ServiceConfigure.PersistenceConfig, typesMap.ToArray());

            return SessionHelper.sessionFectory;
        }
    }

}