using GQService.com.gq.dto;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GQService.com.gq.service
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IStatelessSessionMapper<T> : SessionMapper<T> where T : class, IEntity, new()
    {
        /// <summary>
        /// 
        /// </summary>
        public IStatelessSession session { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        public IStatelessSessionMapper(IStatelessSession session) : base()
        {
            this.session = session;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override T Insert(T value)
        {
            session.Insert(value);
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public override bool Insert(IEnumerable<T> values)
        {
            foreach (var item in values)
            {
                session.Insert(item);
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override T Update(T value)
        {
            session.Update(value);
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public override bool Update(IEnumerable<T> values)
        {
            foreach (var item in values)
            {
                session.Update(item);
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool Delete(T value)
        {
            session.Delete(value);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public override bool Delete(IEnumerable<T> values)
        {
            foreach (var item in values)
            {
                session.Delete(item);
            }
            return true;
        }

        #region Métodos de Consulta

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override T findById(long id)
        {
            return session.Get<T>(id); ;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IQueryable<T> findBy()
        {
            return session.Query<T>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public override IQueryable<T> findBy(System.Linq.Expressions.Expression<Func<T, bool>> expression)
        {
            return session.Query<T>().Where(expression);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public override T findByOne(System.Linq.Expressions.Expression<Func<T, bool>> expression)
        {
            return session.Query<T>().Where(expression).FirstOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public override ISQLQuery findBySql(string sql)
        {
            return session.CreateSQLQuery(sql);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="returnAlias"></param>
        /// <param name="returnClass"></param>
        /// <returns></returns>
        public override ISQLQuery findBySql(string sql, string returnAlias, Type returnClass)
        {
            return session.CreateSQLQuery(sql).AddEntity(returnAlias, returnClass);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="returnAlias"></param>
        /// <param name="returnClass"></param>
        /// <returns></returns>
        public override ISQLQuery findBySql(string sql, string[] returnAlias, Type[] returnClass)
        {
            ISQLQuery result = session.CreateSQLQuery(sql);
            for (int i = 0; i < returnAlias.Length; i++)
            {
                result.AddEntity(returnAlias[i], returnClass[i]);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override long Count()
        {
            return session.Query<T>().Count();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public override long Count(System.Linq.Expressions.Expression<Func<T, bool>> expression)
        {
            return session.Query<T>().Where(expression).Count();
        }

        #endregion
    }
}
