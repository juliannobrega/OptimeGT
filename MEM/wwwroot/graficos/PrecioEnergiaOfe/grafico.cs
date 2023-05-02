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
    
    
    public IList<ComboBoxDto> ObtenerContrato(string baseDatos)
    {
        var sql = Services.session.CreateSQLQuery(string.Format(@"
            SELECT DISTINCT ID_Contrato AS Id, ID_Contrato AS Label
            FROM `{0}`.mod_oferente
            where ID_contrato <> 'SP'
            GROUP BY ID_Contrato", baseDatos)
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

    private IList<GraficoDto> GetDatos(string baseDatos, string contrato, string fechaMin, string fechaMax)
    {
        var sql = Services.session.CreateSQLQuery("");

        if (contrato == "0")
        { 
                    sql = Services.session.CreateSQLQuery(string.Format(@"
                    SELECT Nombre, PrecioEnergia from (
                    SELECT ID_Oferente, round(avg(PrecioEnergia)*1000,2) as PrecioEnergia
                    FROM `{0}`.mod_oferentemes
                    where A * 100 + Mes >= :FechaMin and A * 100 + Mes <= :FechaMax and PrecioESinIndex > 0
                    group by ID_Oferente) t0
                    inner join (select Nombre, ID_Oferente from `{0}`.mod_oferente) as t1 on t0.ID_Oferente = t1.ID_Oferente
                    group by Nombre
                    order by PrecioEnergia desc;
                    ", baseDatos));
        }
        else
        {
            sql = Services.session.CreateSQLQuery(string.Format(@"
                    SELECT Nombre, PrecioEnergia from (
                    SELECT ID_Oferente, round(avg(PrecioEnergia)*1000,2) as PrecioEnergia
                    FROM `{0}`.mod_oferentemes
                    where A * 100 + Mes >= :FechaMin and A * 100 + Mes <= :FechaMax and PrecioESinIndex > 0 and ID_Contrato = :ID_Contrato
                    group by ID_Oferente) t0
                    inner join (select Nombre, ID_Oferente from `{0}`.mod_oferente) as t1 on t0.ID_Oferente = t1.ID_Oferente
                    group by Nombre
                    order by PrecioEnergia desc;
                    ", baseDatos));
                sql.SetParameter("ID_Contrato", contrato);
        }
        
        sql.SetParameter("FechaMin", fechaMin);
        sql.SetParameter("FechaMax", fechaMax);
        sql.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(GraficoDto)));

        return sql.List<GraficoDto>();
    }
    
    
  public object GetExcel(string baseDatos, string contrato, string fechaMin, string fechaMax)
    {
        var list = GetDatos(baseDatos, contrato, fechaMin, fechaMax);

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
    
    
    public object ObtenerGraficos(string baseDatos, string contrato, string fechaMin, string fechaMax)
    {
        ChartCollectionDto cc = new ChartCollectionDto();
        List<ChartDto> charts = cc.Charts;

        var result = GetDatos(baseDatos, contrato, fechaMin, fechaMax);

        try
        {
            var factor = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(result.Count) / 1000));
            Double maxValue = 0;
           
            var label = "PrecioEnergia";
            charts.Add(new ChartDto { key = label, yAxis = 1, type = ChartDto.TYPE_BAR, color = "#1f497d", order = 1 });
            for (int i = 0; i < (result.Count); i += factor)
            {
                Double Value = 0;
                for (int j = i; j < i + factor; j++)
                {
                    Value += result[i].PrecioEnergia;
                }
                charts[charts.Count - 1].values.Add(new ChartValuesDto { x = i , y = Math.Round(Value / factor, 2) });
                GetLabelOferente(result[i], cc.LabelsX, i);
                
            }

        cc.Charts = cc.Charts.OrderBy(x => x.order).ToList();

            return cc;
        }
        catch (Exception)
        {
            return cc;
        }
    }
    
    public string GetLabelOferente(GraficoDto item, Dictionary<string, string> labels, int i)
    {
        if (!labels.ContainsKey(i.ToString()))
        {
            labels.Add(i.ToString(), item.Nombre);
        }
        return labels[i.ToString()];
    }
    

    public class GraficoDto
    {
        //public long A { get; set; }
        //public long Mes { get; set; }
        public string Nombre { get; set; }
        public double PrecioEnergia { get; set; }
        //public double PrecioEne { get; set; }
    }
}
