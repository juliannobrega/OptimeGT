using MEMDataService.com.gq.service.codegen;
using NHibernate;

namespace MEMDataService.com.gq.service
{
    public class ServGq_smtp_config : _ServGq_smtp_config
    {
        #region Constructores

        public ServGq_smtp_config(ISession session): base(session){ }
        public ServGq_smtp_config(IStatelessSession statelessSession): base(statelessSession){ }
        public ServGq_smtp_config(ISession session, IStatelessSession statelessSession): base(session,statelessSession){ }

        #endregion
    }
}
