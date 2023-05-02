using GQService.com.gq.structureMap;
using GQService.com.gq.utils;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace GQService.com.gq.dto
{
    /// <summary>
    /// 
    /// </summary>
    public static class DtoConfiguration
    {
        private static Dictionary<string, Dictionary<string, PropertyInfo>> _objectpropertySet = null;
        /// <summary>
        /// 
        /// </summary>
        public static Dictionary<string, Dictionary<string, PropertyInfo>> objectpropertySet
        {
            get
            {
                if (_objectpropertySet == null || _objectpropertySet.Count == 0)
                {
                    _objectpropertySet = ObjectFactory.GetNamedInstance<Dictionary<string, Dictionary<string, PropertyInfo>>>("objectpropertySet");
                    if (_objectpropertySet == null)
                        _objectpropertySet = new Dictionary<string, Dictionary<string, PropertyInfo>>();

                }
                return _objectpropertySet;
            }
            private set
            {
                _objectpropertySet = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Configure()
        {
            objectpropertySet = new Dictionary<string, Dictionary<string, PropertyInfo>>();

            Type[] types = null;

            types = ClassUtils.getTypes(typeof(Dto<,>));
            setTypes(types);

            ObjectFactory.Configure(x => x.For<Dictionary<string, Dictionary<string, PropertyInfo>>>().Singleton().Use(objectpropertySet).Named("objectpropertySet"));

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="types"></param>
        public static void setTypes(Type[] types)
        {
            foreach (Type type in types)
            {
                foreach (Type t in type.GetTypeInfo().GetGenericArguments())
                {
                    setTypes(t);
                }
                setTypes(type);
            }
        }

        private static object setTypesLock = new object();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Dictionary<string, PropertyInfo> setTypes(Type t)
        {
            string name = t.Namespace + "." + t.Name;

            lock (setTypesLock)
            {
                if (objectpropertySet.ContainsKey(name) == false)
                {
                    objectpropertySet.Add(name, new Dictionary<string, PropertyInfo>());
                    var properties = t.GetProperties();
                    foreach (PropertyInfo porperty in properties)
                    {
                        if (!objectpropertySet[name].ContainsKey(porperty.Name))
                            objectpropertySet[name].Add(porperty.Name, porperty);
                    }
                }
            }
            return objectpropertySet[name];

        }
    }
}
