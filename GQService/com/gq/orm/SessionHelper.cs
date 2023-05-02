using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using System;

namespace GQService.com.gq.orm
{
    /// <summary>
    /// 
    /// </summary>
    public static class SessionHelper
    {
        /// <summary>
        /// 
        /// </summary>
        public static ISessionFactory sessionFectory { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="mapObject"></param>
        public static void BuildSessionFactory(IPersistenceConfigurer config, params Type[] mapObject)
        {
            if (sessionFectory == null)
            {
                try
                {
                    FluentConfiguration fluentConfiguration = Fluently.Configure();
                    foreach(Type item in mapObject)
                    {
                        fluentConfiguration.Mappings(m => m.FluentMappings.Add(item));
                    }

                    sessionFectory = fluentConfiguration
                     .Database( config )
                     .BuildSessionFactory();
                }
                catch (Exception ex)
                {
                    if (ex.InnerException == null)
                    {
                        throw ex;
                    }
                    else
                    {
                        throw ex.InnerException;
                    }
                }
            }
        }
    }
}

