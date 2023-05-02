using Microsoft.AspNetCore.Http;
using StructureMap;
using StructureMap.Pipeline;
using System.Collections.Generic;

namespace GQService.com.gq.structureMap
{
    public class AspNetCoreLifecycle :LifecycleBase,  ILifecycle
    {

        private readonly object mapLock = new object();

        public string Description => "Asp Net Core Lifecycle object";

        private readonly IContainer container;
        private Dictionary<HttpContext, IObjectCache> contextMap = new Dictionary<HttpContext, IObjectCache>();

        public AspNetCoreLifecycle(IContainer cont):base()
        {
            this.container = cont;

        }

        public override void EjectAll(ILifecycleContext context)
        {

            lock (mapLock)
            {
                foreach (var kvp in contextMap)
                {
                    kvp.Value.DisposeAndClear();
                }

                contextMap = new Dictionary<HttpContext, IObjectCache>();
            }
        }

        public override IObjectCache FindCache(ILifecycleContext context)
        {

            var accessor = container.GetInstance<IHttpContextAccessor>();

            lock (mapLock)
            {
                if (!contextMap.ContainsKey(accessor.HttpContext))
                {
                    contextMap.Add(accessor.HttpContext, new LifecycleObjectCache());
                }

                return contextMap[accessor.HttpContext];
            }
        }
    }
}