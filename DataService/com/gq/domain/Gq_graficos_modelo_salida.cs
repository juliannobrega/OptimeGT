using System;

namespace MEMDataService.com.gq.domain
{
    public class Gq_graficos_modelo_salida
    {
        public string ID_Gasoducto { get; set; }

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
        
    }
}
