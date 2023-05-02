using NHibernate;
using System;

namespace GQService.com.gq.service
{
    /// REVISAR
    ///http://ayende.com/blog/4137/nhibernate-perf-tricks
    
    /// <summary>
    /// Clase base de servicios de datos para NHibernate
    /// </summary>
    public class BaseService : IDisposable
    {
        /// <summary>
        /// Objeto de sesion de la base de datos
        /// </summary>
        public virtual ISession Session { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual IStatelessSession StatelessSession { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        public BaseService(ISession session)
        {
            Session = session;
        }

        /// <summary>
        /// *
        /// </summary>
        /// <param name="statelessSession"></param>
        public BaseService(IStatelessSession statelessSession)
        {
            StatelessSession = statelessSession;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="statelessSession"></param>
        public BaseService(ISession session,IStatelessSession statelessSession)
        {
            Session = session;
            StatelessSession = statelessSession;
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            //revisar codigo
            return;
            try
            {
                if (Session != null)
                {
                    if (Session.Transaction != null)
                        Session.Transaction.Dispose();
                    Session.Dispose();
                }
            }
            catch { }
            try
            {
                if (StatelessSession != null)
                {
                    if (StatelessSession.Transaction != null)
                        StatelessSession.Transaction.Dispose();
                    StatelessSession.Dispose();
                }
            }
            catch { }
        }
    }
}
