
using MEMDataService.com.gq.service.codegen;
using NHibernate;

namespace MEMDataService.com.gq.service
{
    public class ServGq_accesos : _ServGq_accesos
    {
    	#region Constructores

        public ServGq_accesos(ISession session): base(session){}
        public ServGq_accesos(IStatelessSession statelessSession): base(statelessSession){}
        public ServGq_accesos(ISession session, IStatelessSession statelessSession): base(session,statelessSession){}

        #endregion
    }
}
