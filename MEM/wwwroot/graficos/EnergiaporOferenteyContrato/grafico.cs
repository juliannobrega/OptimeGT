using GQService.com.gq.dto;
using GQService.com.gq.excel;
using GQService.com.gq.service;
using MEM.com.gq.graficos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Main
{
    public ComboBoxDto fistValue = new ComboBoxDto { Id = "0", Label = "Todos" };

    public IList<ComboBoxDto> ObtenerNombre(string baseDatos)
    {
        var sql = Services.session.CreateSQLQuery(string.Format(@"                     
            SELECT DISTINCT Nombre AS Id, Nombre AS Label
            FROM `{0}`.mod_oferente            
            GROUP BY Nombre", baseDatos)
        );
        sql.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(ComboBoxDto)));
        var result = sql.List<ComboBoxDto>();
        result.Insert(0, fistValue);
        return result;
    }

    public IList<ComboBoxDto> ObtenerFechas(string baseDatos)
    {
        var sql = Services.session.CreateSQLQuery(string.Format(@"
            SELECT concat(CONVERT(A,char), CONVERT(RIGHT(100+ Mes,2) , char)) as Id, concat(CONVERT(A,char), ' ' , CONVERT(RIGHT(100+ Mes,2) , char)) as Label
            FROM `{0}`.mod_oferentemes
            GROUP BY concat(CONVERT(A,char), ' ' , CONVERT(Mes , char)) 
            ORDER BY A, Mes
            ", baseDatos)
        );
        sql.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(ComboBoxDto)));
        return sql.List<ComboBoxDto>();
    }

    private IList<GraficoDto> GetDatos(string baseDatos, string nombre, string fechaMin, string fechaMax)
    {
        var sql = Services.session.CreateSQLQuery("");

        if (nombre == "0")
        {
            sql = Services.session.CreateSQLQuery(string.Format(@"
            SELECT T2.A,t2.Mes,t2.ID_Hora,
            OC,DCC,SE,PerfilAsignadoOV,
            PerfilPorcentual*PotenciaConEnergiaAsociada as 'EnergiaLicitacion'
            FROM 
            (SELECT A,Mes,T0.ID_Mes,ID_Hora,
            sum(if(t0.id_contrato='OC',PerfilAsignado,0)) as 'OC',
            sum(if(t0.id_contrato='DCC',PerfilAsignado,0)) as DCC,
            sum(if(t0.id_contrato='SE',PerfilAsignado,0)) as SE
            from `{0}`.mod_oferentePerfil as t0
                inner join `{0}`.mod_oferente as t3 on t0.ID_Oferente=t3.ID_Oferente group by A,Mes,ID_Hora) t2
                inner join `{0}`.mod_licitacionmes as t4 on t2.Id_Mes=T4.ID_Mes
                inner join `{0}`.mod_licitacionperfil as t5 on t2.Id_Mes=T5.ID_Mes and t2.ID_Hora=t5.ID_Hora
            where (t2.A*100+t2.Mes>= :FechaMin and t2.A*100+t2.Mes<= :FechaMax)
            group by T2.A,t2.Mes,T2.ID_Hora
            order by T2.A,t2.Mes,T2.ID_Hora
            ", baseDatos));
        }
        else
        {
            sql = Services.session.CreateSQLQuery(string.Format(@"
            SELECT T2.A,t2.Mes,t2.ID_Hora,
            OC,DCC,SE,0 as 'PerfilAsignadoOV',
            PerfilPorcentual*PotenciaConEnergiaAsociada as 'EnergiaLicitacion'
            FROM 
            (SELECT A,Mes,T0.ID_Mes,ID_Hora,
            sum(if(t0.id_contrato='OC',PerfilAsignado,0)) as 'OC',
            sum(if(t0.id_contrato='DCC',PerfilAsignado,0)) as DCC,
            sum(if(t0.id_contrato='SE',PerfilAsignado,0)) as SE
            from `{0}`.mod_oferentePerfil as t0
                inner join (select * from `{0}`.mod_oferente where Nombre= :Nombre) as t3 on t0.ID_Oferente=t3.ID_Oferente group by A,Mes,ID_Hora) t2
                inner join `{0}`.mod_licitacionmes as t4 on t2.Id_Mes=T4.ID_Mes
                inner join `{0}`.mod_licitacionperfil as t5 on t2.Id_Mes=T5.ID_Mes and t2.ID_Hora=t5.ID_Hora
            where (t2.A*100+t2.Mes>= :FechaMin and t2.A*100+t2.Mes<= :FechaMax)
            group by T2.A,t2.Mes,T2.ID_Hora
            order by T2.A,t2.Mes,T2.ID_Hora
            ", baseDatos));
            sql.SetParameter("Nombre", nombre);
        }

        sql.SetParameter("FechaMin", fechaMin);
        sql.SetParameter("FechaMax", fechaMax);
        sql.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(GraficoDto)));

        return sql.List<GraficoDto>();
    }

    public object GetExcel(string baseDatos, string nombre, string fechaMin, string fechaMax)
    {
        var list = GetDatos(baseDatos, nombre, fechaMin, fechaMax);

        Dictionary<string, string> labels = new Dictionary<string, string>();

        var workbook = new Workbook();
        workbook.Worksheets.Add(new Worksheet("Datos"));
        var worksheet = workbook.Worksheets[0];

        var ws = worksheet;

        ws.GenerateByIEnumerable(list);

        byte[] bytes;
        using (MemoryStream oStream = new MemoryStream())
        {
            workbook.Save(oStream);
            oStream.Position = 0;
            bytes = oStream.ToArray();
        }
        var file = new FileContentResult(bytes, "application/octet-stream");
        return file;
    }

    public object ObtenerGraficos(string baseDatos, string nombre, string fechaMin, string fechaMax)
    {
        ChartCollectionDto cc = new ChartCollectionDto();
        List<ChartDto> charts = cc.Charts;

        var result = GetDatos(baseDatos, nombre, fechaMin, fechaMax);

        try
        {
            var factor = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(result.Count) / 1000));
            Double maxValue = 0;
            Double minValue = int.MaxValue;
            Double ValorX = 0;
            Double ValueY = 0;

            for (int i = 0; i < (result.Count); i += factor)
            {
                ValorX = ((result[i].A * 10000) + (result[i].Mes * 100) + result[i].ID_Hora);
                cc.LabelsX.Add(i.ToString(), ValorX.ToString());

                //valores maximo y minimo
                ValueY = 0;
                for (int j = i; j < i + factor; j++)
                {
                    ValueY += result[i].EnergiaLicitacion;
                }
                if (maxValue < Math.Round(ValueY / factor, 2))
                {
                    maxValue = Math.Round(ValueY / factor, 2);
                }
                if (minValue > Math.Round(ValueY / factor, 2))
                {
                    minValue = Math.Round(ValueY / factor, 2);
                }
            }

            cc.YmaxValue = maxValue < 0 ? 0 : maxValue;
            cc.YminValue = minValue > 0 ? 0 : minValue;

            var label = "DCC";
            charts.Add(new ChartDto { key = label, yAxis = 1, type = ChartDto.TYPE_AREA, color = "#1f497d", order = 1 });
            for (int i = 0; i < (result.Count); i += factor)
            {
                Double Value = 0;
                for (int j = i; j < i + factor; j++)
                {
                    Value += result[i].DCC;
                }
                charts[charts.Count - 1].values.Add(new ChartValuesDto { x = i + factor, y = Math.Round(Value / factor, 2) });
            }

            label = "OC";
            charts.Add(new ChartDto { key = label, yAxis = 1, type = ChartDto.TYPE_AREA, color = "#FF8C00", order = 2 });
            for (int i = 0; i < (result.Count); i += factor)
            {
                Double Value = 0;
                for (int j = i; j < i + factor; j++)
                {
                    Value += result[i].OC;
                }
                charts[charts.Count - 1].values.Add(new ChartValuesDto { x = i + factor, y = Math.Round(Value / factor, 2) });
            }

            label = "SE";
            charts.Add(new ChartDto { key = label, yAxis = 1, type = ChartDto.TYPE_AREA, color = "#006400", order = 3 });
            for (int i = 0; i < (result.Count); i += factor)
            {
                Double Value = 0;
                for (int j = i; j < i + factor; j++)
                {
                    Value += result[i].SE;
                }
                charts[charts.Count - 1].values.Add(new ChartValuesDto { x = i + factor, y = Math.Round(Value / factor, 2) });
            }

            label = "PerfilAsignadoOV";
            charts.Add(new ChartDto { key = label, yAxis = 1, type = ChartDto.TYPE_AREA, color = "#696969", order = 4 });
            for (int i = 0; i < (result.Count); i += factor)
            {
                Double Value = 0;
                Value = result[i].PerfilAsignadoOV;
                charts[charts.Count - 1].values.Add(new ChartValuesDto { x = i + factor, y = Math.Round(Value, 2) });
            }

            label = "Energia Licitacion";
            charts.Add(new ChartDto { key = label, yAxis = 1, type = ChartDto.TYPE_LINEA, color = "#000000", order = 5 });
            for (int i = 0; i < (result.Count); i += factor)
            {
                Double Value = 0;
                Value = result[i].EnergiaLicitacion;
                charts[charts.Count - 1].values.Add(new ChartValuesDto { x = i + factor, y = Math.Round(Value, 2) });
            }




            cc.Charts = cc.Charts.OrderBy(x => x.order).ToList();

            return cc;
        }
        catch (Exception)
        {
            return cc;
        }
    }

    public class GraficoDto
    {
        public long A { get; set; }
        public long Mes { get; set; }
        public long ID_Hora { get; set; }
        public double OC { get; set; }
        public double DCC { get; set; }
        public double SE { get; set; }
        public double PerfilAsignadoOV { get; set; }
        public double EnergiaLicitacion { get; set; }

    }
}
