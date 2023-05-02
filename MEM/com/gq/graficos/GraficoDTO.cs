using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MEM.com.gq.graficos
{
    public class GraficoDTO
    {
        public string ID_Gasoducto { get; set; }

        public string ID_Nodo{ get; set; }

        public string Concepto { get; set; }

        public int ID_Fecha { get; set; }

        public int ID_FechaGas { get; set; }

        public int ID_Paso { get; set; }

        public int Anio { get; set; }

        public int Semana { get; set; }

        public string DiaTipico { get; set; }

        public decimal Capacidad_Inf { get; set; }

        public decimal Capacidad_Sup { get; set; }

        public decimal Min { get; set; }

        public decimal Percentil25 { get; set; }

        public decimal Promedio { get; set; }

        public decimal Percentil75 { get; set; }

        public decimal Max { get; set; }

        public string Bloque { get; set; }
    }
}
