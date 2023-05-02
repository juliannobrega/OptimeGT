
using MEMDataService.com.gq.service.codegen;
using NHibernate;

namespace MEMDataService.com.gq.service
{
    public class ServGq_escenarios : _ServGq_escenarios
    {
    	#region Constructores

        public ServGq_escenarios(ISession session): base(session){}
        public ServGq_escenarios(IStatelessSession statelessSession): base(statelessSession){}
        public ServGq_escenarios(ISession session, IStatelessSession statelessSession): base(session,statelessSession){}

        #endregion
    }
}
