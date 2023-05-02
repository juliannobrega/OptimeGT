using GQService.com.gq.utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GQService.com.gq.dto
{
    public class Dto<T, E> : IDto<T, E>
        where T : class, IEntity, new()
        where E : class, IDto<T, E>, new()
    {

        public Dto()
        {
        }

        public Dto(T entity)
        {
            SetEntity(entity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual T GetEntity()
        {
            return findEntity();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> GetEntity(IEnumerable<E> values)
        {
            var result = new List<T>();
            foreach (var item in values)
                result.Add(item.GetEntity());
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual E SetEntity(T value)
        {
            if (value == null)
                return null;

            Type typeValue = value.GetType();
            Dictionary<string, PropertyInfo> entityDictionary = null;
            Dictionary<string, PropertyInfo> dtoDictionary = null;

            string classEntityName = typeValue.Namespace + "." + typeValue.Name;
            string classDtoName = this.GetType().Namespace + "." + this.GetType().Name;

            if (DtoConfiguration.objectpropertySet.ContainsKey(classDtoName))
                dtoDictionary = DtoConfiguration.objectpropertySet[classDtoName];
            else
                dtoDictionary = DtoConfiguration.setTypes(this.GetType());

            if (DtoConfiguration.objectpropertySet.ContainsKey(classEntityName))
                entityDictionary = DtoConfiguration.objectpropertySet[classEntityName];
            else
                entityDictionary = DtoConfiguration.setTypes(typeValue);

            setObjectTo(dtoDictionary, entityDictionary, value);

            return this as E;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public virtual IEnumerable<E> SetEntity(IEnumerable<T> values)
        {
            var t = this.GetType();
            var result = new List<E>();
            foreach (var item in values)
            {
                var obj = (E)Activator.CreateInstance(t);
                result.Add(obj.SetEntity(item));
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public virtual IEnumerable<IEntity> GetEntity(IEnumerable<IGenericDto> values)
        {
            var result = new List<IEntity>();
            foreach (var item in values)
                result.Add(item.GetEntity());
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEntity IGenericDto.GetEntity()
        {
            return findEntity();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual IGenericDto SetEntity(IEntity value)
        {

            Type typeValue = value.GetType();
            Dictionary<string, PropertyInfo> entityDictionary = null;
            Dictionary<string, PropertyInfo> dtoDictionary = null;

            string classEntityName = typeValue.Namespace + "." + typeValue.Name;
            string classDtoName = this.GetType().Namespace + "." + this.GetType().Name;

            if (DtoConfiguration.objectpropertySet.ContainsKey(classEntityName))
                entityDictionary = DtoConfiguration.objectpropertySet[classEntityName];
            else
                entityDictionary = DtoConfiguration.setTypes(typeof(T));

            if (DtoConfiguration.objectpropertySet.ContainsKey(classDtoName))
                dtoDictionary = DtoConfiguration.objectpropertySet[classDtoName];
            else
                dtoDictionary = DtoConfiguration.setTypes(this.GetType());

            setObjectTo(dtoDictionary, entityDictionary, value);

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public virtual IEnumerable<IGenericDto> SetEntity(IEnumerable<IEntity> values)
        {
            var t = this.GetType();
            var result = new List<IGenericDto>();
            foreach (var item in values)
            {
                var obj = (IGenericDto)Activator.CreateInstance(t);
                result.Add(obj.SetEntity(item));
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtoDictionary"></param>
        /// <param name="entityDictionary"></param>
        /// <param name="values"></param>
        private void setObjectTo(Dictionary<string, PropertyInfo> dtoDictionary, Dictionary<string, PropertyInfo> entityDictionary, object values)
        {
            if (dtoDictionary == null || entityDictionary == null || values == null)
                return;

            foreach (string key in entityDictionary.Keys)
            {
                try
                {
                    if (dtoDictionary.ContainsKey(key))
                    {
                        if (dtoDictionary[key].CanWrite == true && entityDictionary[key].CanRead)
                        {
                            if (entityDictionary[key].GetValue(values) is IList)
                            {
                                IGenericDto obj = null;
                                try
                                {
                                    obj = ClassUtils.getNewInstance(ClassUtils.getElementType(dtoDictionary[key].PropertyType)) as IGenericDto;
                                }
                                catch { }
                                if (obj != null)
                                {
                                    var r = obj.SetEntity((IEnumerable<IEntity>)entityDictionary[key].GetValue(values));
                                    var l = ClassUtils.getNewInstance(dtoDictionary[key].PropertyType);

                                    foreach (var item in r)
                                        ((IList)l).Add(item);

                                    dtoDictionary[key].SetValue(this, l);
                                }
                                else
                                {
                                    if (dtoDictionary[key].PropertyType.GetTypeInfo().BaseType.Name == "Array")
                                    {
                                        var val = entityDictionary[key].GetValue(values);
                                        if (val is IPclCloneable)
                                            dtoDictionary[key].SetValue(this, ((IPclCloneable)val).Clone());
                                    }
                                    else
                                    {
                                        var l = ClassUtils.getNewInstance(dtoDictionary[key].PropertyType);
                                        var val = entityDictionary[key].GetValue(values);
                                        foreach (var item in (IList)val)
                                            ((IList)l).Add(((IPclCloneable)item).Clone());

                                        dtoDictionary[key].SetValue(this, l);
                                    }
                                }

                            }
                            else if (entityDictionary[key].GetValue(values) is IEntity)
                            {
                                IGenericDto obj = (IGenericDto)ClassUtils.getNewInstance(dtoDictionary[key].PropertyType);
                                dtoDictionary[key].SetValue(this, obj.SetEntity((IEntity)entityDictionary[key].GetValue(values)));
                            }
                            else
                                dtoDictionary[key].SetValue(this, entityDictionary[key].GetValue(values));
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private T findEntity()
        {
            T entity = new T();

            Dictionary<string, PropertyInfo> entityDictionary = null;
            Dictionary<string, PropertyInfo> dtoDictionary = null;

            string classDtoName = this.GetType().Namespace + "." + this.GetType().Name;
            string classEntityName = entity.GetType().Namespace + "." + entity.GetType().Name;

            if (DtoConfiguration.objectpropertySet.ContainsKey(classEntityName))
                entityDictionary = DtoConfiguration.objectpropertySet[classEntityName];
            else
                entityDictionary = DtoConfiguration.setTypes(typeof(T));

            if (DtoConfiguration.objectpropertySet.ContainsKey(classDtoName))
                dtoDictionary = DtoConfiguration.objectpropertySet[classDtoName];
            else
                dtoDictionary = DtoConfiguration.setTypes(this.GetType());

            foreach (string key in dtoDictionary.Keys)
            {
                if (entityDictionary.ContainsKey(key))
                {
                    if (dtoDictionary[key].GetValue(this) is IList)
                    {
                        IGenericDto obj = null;
                        try
                        {
                            obj = ClassUtils.getNewInstance(ClassUtils.getElementType(dtoDictionary[key].PropertyType)) as IGenericDto;
                        }
                        catch { }

                        if (obj != null)
                        {
                            var r = obj.GetEntity((IEnumerable<IGenericDto>)dtoDictionary[key].GetValue(this));
                            var l = ClassUtils.getNewInstance(entityDictionary[key].PropertyType);

                            foreach (var item in r)
                                ((IList)l).Add(item);

                            entityDictionary[key].SetValue(entity, l);
                        }
                        else
                        {
                            if (dtoDictionary[key].PropertyType.GetTypeInfo().BaseType.Name == "Array")
                            {
                                var val = obj.GetEntity((IEnumerable<IGenericDto>)dtoDictionary[key].GetValue(this));
                                if (val is IPclCloneable)
                                    entityDictionary[key].SetValue(entity, ((IPclCloneable)val).Clone());
                            }
                            else
                            {
                                var l = ClassUtils.getNewInstance(entityDictionary[key].PropertyType);
                                var val = dtoDictionary[key].GetValue(this);
                                foreach (var item in (IList)val)
                                {
                                    if (item is IGenericDto)
                                    {
                                        ((IList)l).Add(((IGenericDto)item).GetEntity());
                                    }
                                    else if (item is IPclCloneable)
                                    {
                                        ((IList)l).Add(((IPclCloneable)item).Clone());
                                    }
                                }

                                entityDictionary[key].SetValue(entity, l);
                            }
                        }
                    }
                    else
                    if (dtoDictionary[key].GetValue(this) is IGenericDto)
                    {
                        var l = ClassUtils.getNewInstance(entityDictionary[key].PropertyType);
                        var val = ((IGenericDto)dtoDictionary[key].GetValue(this)).GetEntity();
                        entityDictionary[key].SetValue(entity, val);
                    }
                    else if (((dtoDictionary[key].GetValue(this) is IEnumerable) == false || (dtoDictionary[key].GetValue(this) is System.String)) &&
                    (dtoDictionary[key].GetValue(this) is IGenericDto) == false)
                    {
                        entityDictionary[key].SetValue(entity, dtoDictionary[key].GetValue(this));
                    }
                }
            }

            return entity;
        }

        public IEnumerable Clone(IEnumerable listToClone)
        {
            return listToClone.Cast<IPclCloneable>().Select(item => ((IPclCloneable)item).Clone()).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public virtual E makeDto(T values)
        {
            E dto = new E();

            dto.SetEntity(values);

            return dto;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public virtual List<E> makeDto(List<T> values)
        {
            List<E> dtoList = new List<E>();

            foreach (T item in values)
            {
                dtoList.Add(makeDto(item));
            }

            return dtoList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public virtual IEnumerable makeDto(IEnumerable values)
        {
            List<E> dtoList = new List<E>();

            foreach (T item in values)
            {
                dtoList.Add(makeDto(item));
            }

            return dtoList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public virtual IGenericDto makeDto(IEntity values)
        {
            E dto = new E();
            dto.SetEntity((T)values);
            return dto;
        }

        /// <summary>
        /// 
        /// </summary>
       /* public virtual string className
        {
            get
            {
                return this.GetType().Namespace + "." + this.GetType().Name;
            }
        }*/

    }
    static class Extensions
    {
        public static IList<T> Clone<T>(this IList<T> listToClone) where T : IPclCloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }
    }


}

public interface IPclCloneable
{
    object Clone();
}

