using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MEM.com.gq.supuestos
{
    public class SupuestoInfo
    {
        public string Id { get; private set; } = Guid.NewGuid().ToString();
        public DateTime Fecha { get; private set; } = DateTime.Now;
        public string Nombre { get; set; }
        public long Total { get; set; }
        public long Index { get; set; }
        public bool Fin { get; set; } = false;
        public bool Error { get; set; } = false;
        public string ErrorMsj { get; set; }       
    }
}
