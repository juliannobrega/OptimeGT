using MEMDataService.com.gq.service.codegen;
using NHibernate;

namespace GQDataService.com.gq.service
{
    public class ServGq_mailTemplate : _ServGq_mailTemplate
    {
        #region Constructores

        public ServGq_mailTemplate(ISession session) : base(session) { }
        public ServGq_mailTemplate(IStatelessSession statelessSession) : base(statelessSession) { }
        public ServGq_mailTemplate(ISession session, IStatelessSession statelessSession) : base(session, statelessSession) { }

        #endregion
    }
}
