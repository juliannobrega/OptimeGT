using GQService.com.gq.dto;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GQService.com.gq.service
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SessionMapper<T> where T : class, IEntity, new()
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual T Insert(T value) {
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public virtual bool Insert(IEnumerable<T> values)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual T Update(T value)
        {
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public virtual bool Update(IEnumerable<T> values)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool Delete(T value)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public virtual bool Delete(IEnumerable<T> values)
        {
            return true;
        }

        #region Métodos de Consulta
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T findById(long id)
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<T> findBy()
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual IQueryable<T> findBy(System.Linq.Expressions.Expression<Func<T, bool>> expression)
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual T findByOne(System.Linq.Expressions.Expression<Func<T, bool>> expression)
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public virtual ISQLQuery findBySql(string sql)
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="returnAlias"></param>
        /// <param name="returnClass"></param>
        /// <returns></returns>
        public virtual ISQLQuery findBySql(string sql, string returnAlias, Type returnClass)
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="returnAlias"></param>
        /// <param name="returnClass"></param>
        /// <returns></returns>
        public virtual ISQLQuery findBySql(string sql, string[] returnAlias, Type[] returnClass)
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual long Count()
        {
            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual long Count(System.Linq.Expressions.Expression<Func<T, bool>> expression)
        {
            return -1;
        }

        #endregion
    }

   
}
