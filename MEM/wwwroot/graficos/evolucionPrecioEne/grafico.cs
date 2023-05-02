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

    public IList<ComboBoxDto> ObtenerNombre(string baseDatos, string contrato)
    {
        var sql = Services.session.CreateSQLQuery("");
        if (contrato == "0")
        {
            sql = Services.session.CreateSQLQuery(string.Format(@"
            SELECT DISTINCT Nombre AS Id, Nombre AS Label
            FROM `{0}`.mod_oferente  
            where ID_Contrato <> 'SP'
            GROUP BY Nombre", baseDatos)
           );
        }
        else {
            sql = Services.session.CreateSQLQuery(string.Format(@"
            SELECT DISTINCT Nombre AS Id, Nombre AS Label
            FROM `{0}`.mod_oferente     
            WHERE ID_Contrato = '{1}'
            GROUP BY Nombre", baseDatos, contrato)
          );
        }
        
        sql.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(ComboBoxDto)));
        var result = sql.List<ComboBoxDto>();
        return result;
    }
    public IList<ComboBoxDto> ObtenerContrato(string baseDatos)
    {
        var sql = Services.session.CreateSQLQuery(string.Format(@"
            SELECT DISTINCT ID_Contrato AS Id, ID_Contrato AS Label
            FROM `{0}`.mod_oferenteperfil where ID_contrato <> 'SP'            
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
    
    private IList<GraficoDto> GetDatos(string baseDatos, string nombre, string fechaMin, string fechaMax)
    {
        var sql = Services.session.CreateSQLQuery(string.Format(@"
                SELECT Mes, A, PrecioESinIndex * 1000 as PrecioESinIndex, PrecioEnergia * 1000 as PrecioEnergia from `{0}`.mod_oferentemes t0
                inner join `{0}`.mod_oferente t1 on t0.ID_Oferente = t1.ID_Oferente   
                where Nombre = :Nombre 
                and A * 100 + Mes >= :FechaMin and A * 100 + Mes <= :FechaMax  
			ORDER BY A, Mes
                    ", baseDatos));
        sql.SetParameter("Nombre", nombre);
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

            var label = "PrecioESinIndex";
            charts.Add(new ChartDto { key = label, yAxis = 1, type = ChartDto.TYPE_LINEA, color = "#1f497d", order = 1 });
            for (int i = 0; i < (result.Count); i += factor)
            {
                Double Value = 0;
                for (int j = i; j < i + factor; j++)
                {
                    Value += result[i].PrecioESinIndex;
                }
                charts[charts.Count - 1].values.Add(new ChartValuesDto { x = i , y = Math.Round(Value / factor, 2) });

            }
                       
            label = "PrecioEnergia";
            charts.Add(new ChartDto { key = label, yAxis = 1, type = ChartDto.TYPE_LINEA, color = "#ff0000", order = 2 });
            for (int i = 0; i < (result.Count); i += factor)
            {
                Double Value = 0;
                for (int j = i; j < i + factor; j++)
                {
                    Value += result[i].PrecioEnergia;
                }
                charts[charts.Count - 1].values.Add(new ChartValuesDto { x = i , y = Math.Round(Value / factor, 2) });                               
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
        public double PrecioESinIndex { get; set; }
        public double PrecioEnergia { get; set; }
    }
}
