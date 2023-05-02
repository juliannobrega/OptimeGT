
using MEMDataService.com.gq.service.codegen;
using NHibernate;

namespace MEMDataService.com.gq.service
{
    public class ServGq_usuarios : _ServGq_usuarios
    {
    	#region Constructores

        public ServGq_usuarios(ISession session): base(session){}
        public ServGq_usuarios(IStatelessSession statelessSession): base(statelessSession){}
        public ServGq_usuarios(ISession session, IStatelessSession statelessSession): base(session,statelessSession){}

        #endregion
    }
}
