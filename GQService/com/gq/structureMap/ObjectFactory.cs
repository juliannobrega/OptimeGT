using Microsoft.Extensions.DependencyInjection;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Threading;

namespace GQService.com.gq.structureMap
{
    /// <summary>
    /// 
    /// </summary>
    public static class ObjectFactory
    {
        private static readonly Lazy<Container> _containerBuilder = new Lazy<Container>(defaultContainer, LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// 
        /// </summary>
        public static IContainer Container
        {
            get { return _containerBuilder.Value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public static IServiceCollection Services { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static Container defaultContainer()
        {
            return new Container(x =>
            {
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configure"></param>
        public static void Configure(Action<ConfigurationExpression> configure)
        {
            Container.Configure(configure);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> GetAllInstances<T>()
        {
            List<T> result = new List<T>(Container.GetAllInstances<T>());
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T GetNamedInstance<T>(string name)
        {
            return Container.TryGetInstance<T>(name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetInstance<T>()
        {
            return Container.TryGetInstance<T>();
        }
    }
}
