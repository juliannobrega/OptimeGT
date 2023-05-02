using Microsoft.AspNetCore.Http;
using StructureMap;
using StructureMap.Pipeline;
using System;
using System.Collections.Generic;

namespace GQService.com.gq.structureMap
{
    public class AspNetCoreSessionLifecycle : LifecycleBase,  ILifecycle
    {
        public static TimeSpan TimeOut = TimeSpan.FromHours(4);

        private readonly object mapLock = new object();

        public string Description => "Asp Net Core Session Lifecycle object";

        private readonly IContainer container;
        private Dictionary<string, SessionObject> contextMap = new Dictionary<string, SessionObject>();

        public AspNetCoreSessionLifecycle(IContainer cont):base()
        {
            this.container = cont;
        }

        public override void EjectAll(ILifecycleContext context)
        {

            lock (mapLock)
            {
                foreach (var kvp in contextMap)
                {
                    kvp.Value.ObjectCahe.DisposeAndClear();
                }

                contextMap = new Dictionary<string, SessionObject>();
            }
        }

        public override IObjectCache FindCache(ILifecycleContext context)
        {

            var accessor = container.GetInstance<IHttpContextAccessor>();

            lock (mapLock)
            {
                if (!contextMap.ContainsKey(accessor.HttpContext.Session.Id))
                {
                    accessor.HttpContext.Session.Set("I", new byte[] { 1 });
                    contextMap.Add(accessor.HttpContext.Session.Id, new SessionObject { TimeOut = 0, ObjectCahe = new LifecycleObjectCache() });
                }

                return contextMap[accessor.HttpContext.Session.Id].ObjectCahe;
            }
        }

        protected class SessionObject
        {
            public long TimeOut { get; set; }
            public IObjectCache ObjectCahe { get; set; }
        }
    }
}