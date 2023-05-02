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
        result.Insert(0, fistValue);
        return result;
    }
    public IList<ComboBoxDto> ObtenerContrato(string baseDatos)
    {
        var sql = Services.session.CreateSQLQuery(string.Format(@"
            SELECT DISTINCT ID_Contrato AS Id, ID_Contrato AS Label
            FROM `{0}`.mod_oferenteperfil            
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

    private IList<GraficoDto> GetDatos(string baseDatos, string nombre, string contrato, string fechaMin, string fechaMax)
    {
        var sql = Services.session.CreateSQLQuery("");

        if (nombre == "0")
        {
            if (contrato == "0")
            {
                //nombres todos 
                //contratos todos 
                sql = Services.session.CreateSQLQuery(string.Format(@"
                SELECT t2.Mes,T2.A,t1.ID_Hora,
                sum(t1.PerfilAsignado) as PerfilAsignado,
                sum(t2.PotenciaAdjudicada) as PotenciaAdjudicada
                FROM  `{0}`.mod_oferenteperfil as t1
                    inner join  `{0}`.mod_oferentemes as t2 on t1.ID_Oferente = t2.ID_Oferente and t1.ID_Mes = t2.ID_Mes
                    inner join  `{0}`.mod_oferente as t3 on t1.ID_Oferente = t3.ID_Oferente
                where t2.A * 100 + t2.Mes >= :FechaMin and t2.A * 100 + t2.Mes <= :FechaMax            
                group by T2.A,t2.Mes,t1.ID_Hora
                order by T2.A,t2.Mes,t1.ID_Hora
                ", baseDatos));
            }
            else
            {
                //nombres todos
                //contratos valor
                sql = Services.session.CreateSQLQuery(string.Format(@"
                    SELECT t2.Mes,T2.A,t1.ID_Hora,
                    sum(t1.PerfilAsignado) as PerfilAsignado,
                    sum(t2.PotenciaAdjudicada) as PotenciaAdjudicada
                    FROM  `{0}`.mod_oferenteperfil as t1
                        inner join  `{0}`.mod_oferentemes as t2 on t1.ID_Oferente = t2.ID_Oferente and t1.ID_Mes = t2.ID_Mes
                        inner join  `{0}`.mod_oferente as t3 on t1.ID_Oferente = t3.ID_Oferente
                    where t1.ID_Contrato = :ID_Contrato                   
                    and t2.A * 100 + t2.Mes >= :FechaMin and t2.A * 100 + t2.Mes <= :FechaMax            
                    group by T2.A,t2.Mes,t1.ID_Hora
                    order by T2.A,t2.Mes,t1.ID_Hora
                    ", baseDatos));
                sql.SetParameter("ID_Contrato", contrato);
            }

        }
        else
        {
            if (contrato == "0")
            {
                //nombres valor 
                //contratos todos 
                sql = Services.session.CreateSQLQuery(string.Format(@"
                SELECT t2.Mes,T2.A,t1.ID_Hora,
                sum(t1.PerfilAsignado) as PerfilAsignado,
                sum(t2.PotenciaAdjudicada) as PotenciaAdjudicada
                FROM  `{0}`.mod_oferenteperfil as t1
                    inner join  `{0}`.mod_oferentemes as t2 on t1.ID_Oferente = t2.ID_Oferente and t1.ID_Mes = t2.ID_Mes
                    inner join  `{0}`.mod_oferente as t3 on t1.ID_Oferente = t3.ID_Oferente
                where t3.Nombre = :Nombre
                and t2.A * 100 + t2.Mes >= :FechaMin and t2.A * 100 + t2.Mes <= :FechaMax
                group by T2.A,t2.Mes,t1.ID_Hora
                order by T2.A,t2.Mes,t1.ID_Hora
                ", baseDatos));
                sql.SetParameter("Nombre", nombre);
            }
            else
            {
                //nombres valor
                //contratos valor
                sql = Services.session.CreateSQLQuery(string.Format(@"
                SELECT t2.Mes,T2.A,t1.ID_Hora,
                sum(t1.PerfilAsignado) as PerfilAsignado,
                sum(t2.PotenciaAdjudicada) as PotenciaAdjudicada
                FROM  `{0}`.mod_oferenteperfil as t1
                    inner join  `{0}`.mod_oferentemes as t2 on t1.ID_Oferente = t2.ID_Oferente and t1.ID_Mes = t2.ID_Mes
                    inner join  `{0}`.mod_oferente as t3 on t1.ID_Oferente = t3.ID_Oferente
                where t1.ID_Contrato = :ID_Contrato
                and t3.Nombre = :Nombre
                and t2.A * 100 + t2.Mes >= :FechaMin and t2.A * 100 + t2.Mes <= :FechaMax            
                group by T2.A,t2.Mes,t1.ID_Hora
                order by T2.A,t2.Mes,t1.ID_Hora
                ", baseDatos));
                sql.SetParameter("ID_Contrato", contrato);
                sql.SetParameter("Nombre", nombre);
            }
        }

        sql.SetParameter("FechaMin", fechaMin);
        sql.SetParameter("FechaMax", fechaMax);
        sql.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(GraficoDto)));

        return sql.List<GraficoDto>();
    }

    public object GetExcel(string baseDatos, string nombre, string contrato, string fechaMin, string fechaMax)
    {
        var list = GetDatos(baseDatos, nombre, contrato, fechaMin, fechaMax);

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

    public object ObtenerGraficos(string baseDatos, string nombre, string contrato, string fechaMin, string fechaMax)
    {
        ChartCollectionDto cc = new ChartCollectionDto();
        List<ChartDto> charts = cc.Charts;

        var result = GetDatos(baseDatos, nombre, contrato, fechaMin, fechaMax);

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
                    ValueY += result[i].PerfilAsignado + result[i].PotenciaAdjudicada;
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

            var label = "Perfil Asignado";
            charts.Add(new ChartDto { key = label, yAxis = 1, type = ChartDto.TYPE_AREA, color = "#1f497d", order = 1 });
            for (int i = 0; i < (result.Count); i += factor)
            {
                Double Value = 0;
                for (int j = i; j < i + factor; j++)
                {
                    Value += result[i].PerfilAsignado;
                }
                charts[charts.Count - 1].values.Add(new ChartValuesDto { x = i + factor, y = Math.Round(Value / factor, 2) });                              
            }

            label = "Potencia Adjudicada";
            charts.Add(new ChartDto { key = label, yAxis = 1, type = ChartDto.TYPE_LINEA, color = "#ff0000", order = 2 });
            for (int i = 0; i < (result.Count); i += factor)
            {
                Double Value = 0;
                for (int j = i; j < i + factor; j++)
                {
                    Value += result[i].PotenciaAdjudicada;
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
        public long ID_Hora { get; set; }
        public double PerfilAsignado { get; set; }
        public double PotenciaAdjudicada { get; set; }
    }
}
