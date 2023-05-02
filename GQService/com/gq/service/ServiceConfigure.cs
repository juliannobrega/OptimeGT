using StructureMap.Web;
using FluentNHibernate.Cfg.Db;
using System.Reflection;

namespace GQService.com.gq.service
{
    /// <summary>
    /// 
    /// </summary>
    public static class ServiceConfigure
    {
        /// <summary>
        /// 
        /// </summary>
        public static void Configure(Assembly assembly,IPersistenceConfigurer config)
        {
            PersistenceConfig = config;
            structureMap.ObjectFactory.Configure(x =>
            {
                // ISessionFactory is expensive to initialize, so create it as a singleton.
                x.For<NHibernate.ISessionFactory>()
                    .Singleton()
                    .Use(Services.GetSessionFactory(assembly));

                // Cache each ISession per web request. Remember to dispose this!
                x.For<NHibernate.ISession>()
                    .HybridHttpOrThreadLocalScoped()
                    .Use(context => context.GetInstance<NHibernate.ISessionFactory>().OpenSession());

                // Cache each IStatelessSession per web request. Remember to dispose this!
                x.For<NHibernate.IStatelessSession>()
                    .HybridHttpOrThreadLocalScoped()
                    .Use(context => context.GetInstance<NHibernate.ISessionFactory>().OpenStatelessSession());
            });

        }

        /// <summary>
        /// 
        /// </summary>
        public static IPersistenceConfigurer PersistenceConfig { get; set; }
    }
}