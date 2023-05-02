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
            SELECT T2.A, t2.Mes,
            sum(if(t2.id_contrato='DCC',t2.potenciaAdjudicada,0)) as DCC,
            sum(if(t2.id_contrato='OC',t2.potenciaAdjudicada,0)) as OC,
            sum(if(t2.id_contrato='SP',t2.potenciaAdjudicada,0)) as SP,
            sum(if(potenciaAdjudicada>0,PotenciaMaxima,0)) as PotenciaMaxima,
            sum(if(potenciaAdjudicada>0,PotenciaMinima,0)) as PotenciaMinima
            FROM `{0}`.mod_oferentemes as t2 
                inner join `{0}`.mod_oferente as t3 on t2.ID_Oferente=t3.ID_Oferente
            where t2.A * 100 + t2.Mes >= :FechaMin and t2.A * 100 + t2.Mes <= :FechaMax
            group by T2.A, t2.Mes
            order by T2.A, t2.Mes
            ", baseDatos));
        }
        else
        {
            sql = Services.session.CreateSQLQuery(string.Format(@"
            SELECT T2.A, t2.Mes,
            sum(if(t2.id_contrato='DCC',t2.potenciaAdjudicada,0)) as DCC,
            sum(if(t2.id_contrato='OC',t2.potenciaAdjudicada,0)) as OC,
            sum(if(t2.id_contrato='SP',t2.potenciaAdjudicada,0)) as SP,
            sum(PotenciaMaxima) as PotenciaMaxima,
            sum(PotenciaMinima) as PotenciaMinima
            FROM `{0}`.mod_oferentemes as t2 
                inner join `{0}`.mod_oferente as t3 on t2.ID_Oferente=t3.ID_Oferente
            where t3.Nombre = :Nombre            
                and t2.A * 100 + t2.Mes >= :FechaMin and t2.A * 100 + t2.Mes <= :FechaMax
            group by T2.A, t2.Mes
            order by T2.A, t2.Mes
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
            Double ValorX = 0;

            for (int i = 0; i < (result.Count); i += factor)
            {
                ValorX = ((result[i].A * 1000) + (result[i].Mes * 10));
                cc.LabelsX.Add(i.ToString(), ValorX.ToString());
            }

            var label = "DCC";
            charts.Add(new ChartDto { key = label, yAxis = 1, type = ChartDto.TYPE_BAR, color = "#1f497d", order = 2 });
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
            charts.Add(new ChartDto { key = label, yAxis = 1, type = ChartDto.TYPE_BAR, color = "#FF8C00", order = 1 });
            for (int i = 0; i < (result.Count); i += factor)
            {
                Double Value = 0;
                for (int j = i; j < i + factor; j++)
                {
                    Value += result[i].OC;
                }
                charts[charts.Count - 1].values.Add(new ChartValuesDto { x = i + factor, y = Math.Round(Value / factor, 2) });

              
            }

            label = "SP";
            charts.Add(new ChartDto { key = label, yAxis = 1, type = ChartDto.TYPE_BAR, color = "#ff0000", order = 3 });
            for (int i = 0; i < (result.Count); i += factor)
            {
                Double Value = 0;
                for (int j = i; j < i + factor; j++)
                {
                    Value += result[i].SP;
                }
                charts[charts.Count - 1].values.Add(new ChartValuesDto { x = i + factor, y = Math.Round(Value / factor, 2) });

            }

            label = "PotenciaMaxima";
            charts.Add(new ChartDto { key = label, yAxis = 1, type = ChartDto.TYPE_LINEA, color = "#ff0000", order = 4 });
            for (int i = 0; i < (result.Count); i += factor)
            {
                Double Value = 0;
                for (int j = i; j < i + factor; j++)
                {
                    Value += result[i].PotenciaMaxima;
                }
                charts[charts.Count - 1].values.Add(new ChartValuesDto { x = i + factor, y = Math.Round(Value / factor, 2) });

            }

            label = "PotenciaMinima";
            charts.Add(new ChartDto { key = label, yAxis = 1, type = ChartDto.TYPE_LINEA, color = "#ff0000", order = 4 });
            for (int i = 0; i < (result.Count); i += factor)
            {
                Double Value = 0;
                for (int j = i; j < i + factor; j++)
                {
                    Value += result[i].PotenciaMinima;
                }
                charts[charts.Count - 1].values.Add(new ChartValuesDto { x = i + factor, y = Math.Round(Value / factor, 2) });
                
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
        public double OC { get; set; }
        public double DCC { get; set; }
        public double SP { get; set; }
        public double PotenciaMaxima { get; set; }
        public double PotenciaMinima { get; set; }

    }
}
