using System;
using System.Collections;
using NHibernate;
using NHibernate.Properties;
using NHibernate.Transform;
using System.Collections.Generic;

namespace MEM.com.gq.supuestos
{
    

    [Serializable]
    public class DictionaryResultTransformer : IResultTransformer
    {

        public DictionaryResultTransformer()
        {

        }

        #region IResultTransformer Members

        public IList TransformList(IList collection)
        {
            return collection;
        }

        public object TransformTuple(object[] tuple, string[] aliases)
        {
            var result = new Dictionary<string, object>();
            for (int i = 0; i < aliases.Length; i++)
            {
                result[aliases[i].ToUpper()] = tuple[i];
            }
            return result;
        }

        #endregion
    }
}
