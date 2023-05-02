
using MEMDataService.com.gq.service.codegen;
using NHibernate;

namespace MEMDataService.com.gq.service
{
    public class ServGq_tipos_grafico : _ServGq_tipos_grafico
    {
    	#region Constructores

        public ServGq_tipos_grafico(ISession session): base(session){}
        public ServGq_tipos_grafico(IStatelessSession statelessSession): base(statelessSession){}
        public ServGq_tipos_grafico(ISession session, IStatelessSession statelessSession): base(session,statelessSession){}

        #endregion
    }
}
