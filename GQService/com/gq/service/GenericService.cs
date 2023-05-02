using GQService.com.gq.dto;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GQService.com.gq.service
{
    /// <summary>
    /// Clase generica con metos basicos de servicios de busqueda en base de datos
    /// </summary>
    /// <typeparam name="T">Entidad de base de datos</typeparam>
    public class GenericService<T> : BaseService where T : class, IEntity, new()
    {
        #region Constructores
        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        public GenericService(ISession session)
            : base(session)
        {
            _SessionMode = new ISessionMapper<T>(session);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="statelessSession"></param>
        public GenericService(IStatelessSession statelessSession)
           : base(statelessSession)
        {
            _SessionMode = new IStatelessSessionMapper<T>(statelessSession);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="statelessSession"></param>
        public GenericService(ISession session, IStatelessSession statelessSession)
           : base(session, statelessSession)
        {
            _SessionMode = new ISessionMapper<T>(session);
        }

        #endregion

        #region Session Mode
        private SessionMapper<T> _SessionMode = null;
        /// <summary>
        /// 
        /// </summary>
        public virtual SessionMapper<T> SessionMode { get { return _SessionMode; } set { _SessionMode = value; } }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual ISessionMapper<T> getISessionMapper()
        {
            return new ISessionMapper<T>(Session);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual IStatelessSessionMapper<T> getIStatelessSessionMapper()
        {
            return new IStatelessSessionMapper<T>(StatelessSession);
        }

        #endregion

        #region Métodos ABM
        /// <summary>
        ///  Agrega una entidad a la base de datos
        /// </summary>
        /// <param name="pObj">Entidad</param>
        /// <returns>Entidad actualizada</returns>
        public virtual T Agregar(T pObj)
        {
            if (_SessionMode != null)
            {
                _SessionMode.Insert(pObj);
            }
            else
            {
                throw new Exception("No tiene session");
            }
            return pObj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pObj"></param>
        /// <returns></returns>
        public virtual bool Agregar(IEnumerable<T> pObj)
        {
            if (_SessionMode != null)
            {
                _SessionMode.Insert(pObj);
            }
            else
            {
                throw new Exception("No tiene session");
            }

            return true;
        }

        /// <summary>
        ///  Actualiza una entidad a la base de datos
        /// </summary>
        /// <param name="pObj">Entidad</param>
        /// <returns>Entidad actualizada</returns>
        public virtual T Actualizar(T pObj)
        {
            if (_SessionMode != null)
            {
                _SessionMode.Update(pObj);
            }
            else
            {
                throw new Exception("No tiene session");
            }
            return pObj;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pObj"></param>
        /// <returns></returns>
        public virtual bool Actualizar(IEnumerable<T> pObj)
        {
            if (_SessionMode != null)
            {
                _SessionMode.Update(pObj);
            }
            else
            {
                throw new Exception("No tiene session");
            }
            return true;
        }

        /// <summary>
        ///  Borra una entidad a la base de datos
        /// </summary>
        /// <param name="pObj">Entidad</param>
        /// <returns>True = se pudo borrar</returns>
        public virtual bool Borrar(T pObj)
        {
            if (_SessionMode != null)
            {
                _SessionMode.Delete(pObj);
            }
            else
            {
                throw new Exception("No tiene session");
            }
            return true;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pObj"></param>
        /// <returns></returns>
        public virtual bool Borrar(IEnumerable<T> pObj)
        {
            if (_SessionMode != null)
            {
                _SessionMode.Delete(pObj);
            }
            else
            {
                throw new Exception("No tiene session");
            }
            return true;
        }
        #endregion

        #region Métodos de Consulta

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public bool Exist(System.Linq.Expressions.Expression<Func<T, bool>> expression)
        {
            return Count(expression) > 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T findById(long? id)
        {
            T result = null;
            if (_SessionMode != null)
            {
                result = _SessionMode.findById(id.Value);
            }
            else
            {
                throw new Exception("No tiene session");
            }
            return result;
        }

        /// <summary>
        /// Busca Las entidaddes 
        /// </summary>
        /// <returns>Lista con los resultados</returns>
        public virtual IQueryable<T> findBy()
        {
            IQueryable<T> result = null;

            if (_SessionMode != null)
            {
                result = _SessionMode.findBy();
            }
            else
            {
                throw new Exception("No tiene session");
            }

            return result;
        }

        /// <summary>
        /// Busca los registros
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <returns></returns>
        public virtual IQueryable<T> findBy(System.Linq.Expressions.Expression<Func<T, bool>> expression)
        {
            IQueryable<T> result = null;

            if (_SessionMode != null)
            {
                result = _SessionMode.findBy(expression);
            }
            else
            {
                throw new Exception("No tiene session");
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual T findByOne(System.Linq.Expressions.Expression<Func<T, bool>> expression)
        {
            T result = null;
            if (_SessionMode != null)
            {
                result = _SessionMode.findByOne(expression);
            }
            else
            {
                throw new Exception("No tiene session");
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public ISQLQuery findBySql(string sql)
        {
            ISQLQuery result = null;
            if (_SessionMode != null)
            {
                result = _SessionMode.findBySql(sql);
            }
            else
            {
                throw new Exception("No tiene session");
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="returnAlias"></param>
        /// <param name="returnClass"></param>
        /// <returns></returns>
        public ISQLQuery findBySql(string sql, string returnAlias, Type returnClass)
        {
            ISQLQuery result = null;
            if (_SessionMode != null)
            {
                result = _SessionMode.findBySql(sql, returnAlias, returnClass);
            }
            else
            {
                throw new Exception("No tiene session");
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="returnAlias"></param>
        /// <param name="returnClass"></param>
        /// <returns></returns>
        public ISQLQuery findBySql(string sql, string[] returnAlias, Type[] returnClass)
        {
            ISQLQuery result = null;
            if (_SessionMode != null)
            {
                result = _SessionMode.findBySql(sql, returnAlias, returnClass);
            }
            else
            {
                throw new Exception("No tiene session");
            }
            return result;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public long Count()
        {
            long result = -1;
            if (_SessionMode != null)
            {
                result = _SessionMode.Count();
            }
            else
            {
                throw new Exception("No tiene session");
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public long Count(System.Linq.Expressions.Expression<Func<T, bool>> expression)
        {
            long result = -1;
            if (_SessionMode != null)
            {
                result = _SessionMode.Count(expression);
            }
            else
            {
                throw new Exception("No tiene session");
            }
            return result;
        }

        #endregion
    }
}
