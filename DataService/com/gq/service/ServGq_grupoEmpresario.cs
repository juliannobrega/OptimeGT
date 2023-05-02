
using MEMDataService.com.gq.service.codegen;
using NHibernate;

namespace MEMDataService.com.gq.service
{
    public class ServGq_grupoEmpresario : _ServGq_grupoEmpresario
    {
    	#region Constructores

        public ServGq_grupoEmpresario(ISession session): base(session){}
        public ServGq_grupoEmpresario(IStatelessSession statelessSession): base(statelessSession){}
        public ServGq_grupoEmpresario(ISession session, IStatelessSession statelessSession): base(session,statelessSession){}

        #endregion
    }
}
