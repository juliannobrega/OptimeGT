
using MEMDataService.com.gq.service.codegen;
using NHibernate;

namespace MEMDataService.com.gq.service
{
    public class ServGq_descargas : _ServGq_descargas
    {
    	#region Constructores

        public ServGq_descargas(ISession session): base(session){}
        public ServGq_descargas(IStatelessSession statelessSession): base(statelessSession){}
        public ServGq_descargas(ISession session, IStatelessSession statelessSession): base(session,statelessSession){}

        #endregion
    }
}
