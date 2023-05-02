using GQService.com.gq.dto;
using GQService.com.gq.excel;
using GQService.com.gq.service;
using MEM.com.gq.graficos;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GQService.com.gq.data;
using GQService.com.gq.log;
using MEM.com.gq.supuestos;
using System.Threading.Tasks;

public class Main
{
    
    private int fila = 0;
    private int columna = 0;
    private string hoja = string.Empty;
    private bool errorUpdate = false;

  
    public IList<ComboBoxDto> BuscarTablasPorSchema(string baseDatos)
    {
        try
        {
            var query = Services.session.CreateSQLQuery(string.Format(@"SELECT table_name AS Id, table_name AS Label  
                                                      FROM information_schema.tables
                                                      WHERE table_schema = '{0}'", baseDatos));
            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(ComboBoxDto)));
            return query.List<ComboBoxDto>();
        }
        catch (Exception ex)
        {
            Log.Error("BuscarTablasPorSchema " + baseDatos, ex);
            return null;
        }

    }

    public IList<object> Buscar(string baseDatos, string sup)
    {
        try
        {
            var query = Services.statelessSession.CreateSQLQuery(string.Format(@"
            SELECT *
            FROM `{0}`.{1}  
            ", baseDatos, sup)).List<object>();
            return query;
        }
        catch (Exception ex)
        {
            Log.Error("Supuesto sup_ImpExpTodos - Buscar", ex);
            return null;
        }

    }

    public object GetExcelBD(string baseDatos)
    {
        try
        {
            var listaSup = BuscarTablasPorSchema(baseDatos);

            byte[] bytes;
            using (var package = new ExcelPackage())
            {
                foreach (var sup in listaSup)
                {
                    try
                    {
                        if (sup.Label.ToString() == "sis_log" || sup.Label.ToString() == "sis_tablasescritura" || sup.Label.ToString() == "res_resultado")
                        {
                            continue;
                        }
                        //buscar supuesto                        
                        var list = Buscar(baseDatos, sup.Label.ToString());

                        if (list != null)
                        {
                            var labelsResult = Services.statelessSession.CreateSQLQuery(string.Format(@"
                                            SELECT `COLUMN_NAME` 
                                            FROM `INFORMATION_SCHEMA`.`COLUMNS` 
                                            WHERE `TABLE_SCHEMA`= '{0}'
                                            AND `TABLE_NAME`='{1}'
                                            ORDER BY `ORDINAL_POSITION`
                                            ", baseDatos, sup.Label.ToString())).List<string>();

                            var columnsType = Services.statelessSession.CreateSQLQuery(string.Format(@"
                                        SELECT `COLUMN_TYPE`
                                        FROM `INFORMATION_SCHEMA`.`COLUMNS` 
                                        WHERE `TABLE_SCHEMA`= '{0}'
                                        AND `TABLE_NAME`='{1}'
                                        ORDER BY `ORDINAL_POSITION`
                                        ", baseDatos, sup.Label.ToString())).List<string>();



                            //cargar hojas excel
                            #region Hoja excel 
                            var ws = package.Workbook.Worksheets.Add(sup.Label.ToString());

                            int row = 1;
                            int col = 1;

                            //header
                            for (var i = 0; i < labelsResult.Count(); i++)
                            {
                                ws.Cells[row, col].Value = labelsResult[i];
                                ws.Cells[row, col].Style.Font.Bold = true;
                                col++;
                            }
                            row++;

                            foreach (var item in list)
                            {
                                col = 1;
                                foreach (var valor in (IEnumerable)item)
                                {
                                    ws.Cells[row, col].Value = valor;
                                    col++;
                                }
                                row++;
                            }

                            ws.Cells.AutoFitColumns();
                            //formato de columnas
                            for (int i = 0; i < columnsType.Count; i++)
                            {
                                var type = columnsType[i];
                                if (type.Contains("int"))
                                {
                                    ws.Column(i + 1).Style.Numberformat.Format = "0";
                                }
                                if (type.Contains("decimal"))
                                {
                                    string afterComma = "0";
                                    if (type.Contains(","))
                                    {
                                        string afterstr = type.ToString().Split(',')[1].ToString().Split(')')[0];
                                        int after = Int32.Parse(afterstr);
                                        for (int j = 1; j < after; j++)
                                        {
                                            afterComma += "0";
                                        }
                                    }
                                    ws.Column(i + 1).Style.Numberformat.Format = "0." + afterComma;
                                }
                                if (type.Contains("double"))
                                {
                                    //por lo general los doubles tienen muchos decimales por defecto
                                    string afterComma = "00000";
                                    if (type.Contains(","))
                                    {
                                        afterComma = "0";
                                        string afterstr = type.ToString().Split(',')[1].ToString().Split(')')[0];
                                        int after = Int32.Parse(afterstr);
                                        for (int j = 1; j < after; j++)
                                        {
                                            afterComma += "0";
                                        }
                                    }
                                    ws.Column(i + 1).Style.Numberformat.Format = "0." + afterComma;
                                }
                            }
                            #endregion
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Get excel - Tablas Licitacion ", ex);
                        //Continia la iteracion
                    }

                }

                using (MemoryStream oStream = new MemoryStream())
                {
                    package.SaveAs(oStream);
                    oStream.Position = 0;
                    bytes = oStream.ToArray();
                }

            }
            var file = new FileContentResult(bytes, "application/octet-stream");
            file.FileDownloadName = baseDatos + "_sup_ImpExpTodos_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx";
            return file;
        }
        catch (Exception ex)
        {
            Log.Error("Get excel - Tablas Licitacion ", ex);

        }

        return null;
    }

    public class SchemaInformation
    {
        public string COLUMN_NAME { get; set; }
        public string DATA_TYPE { get; set; }
    }
    
    public ComboBoxDto fistValue = new ComboBoxDto { Id = "0", Label = "Todos" };

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

    public object GetExcelRes(string baseDatos)
    {
        byte[] bytes;
        using (var package = new ExcelPackage())
        {
            //buscar supuesto
            #region BuscarSupuesto

            var ws = package.Workbook.Worksheets.Add("TablaResultado");
            var sup = "res_resultado";
            //var list = Buscar(baseDatos, sup.Label.ToString());
            var list = Services.statelessSession.CreateSQLQuery(string.Format(@"
            SELECT *
            FROM `{0}`.{1}  
            ORDER BY 
                case when ( ID_Oferente+0 = 0) then
				    case when(ID_Oferente = 'Total Neto OV') then 1000 else 999 end
                else ID_Oferente+0 
		        end;
            ", baseDatos, sup)).List<object>();


            var labelsResult = new string[] {   "Oferente",
                                                "Nombre",
                                                "Contrato",
                                                "Tipo",
                                                "Flexibilidad",
                                                "Precio Potencia (USD/kW-Mes)",
                                                "Precio Energia (USD/MWh)",
                                                "Energia Asignada (MWh)",
                                                "Potencia Media Ofertada (kW)",
                                                "Potencia Media Adjudicada (kW)",
                                                "Periodos de Potencia Adjudicada",
                                                "Periodos de Energia Asignada",
                                                "Costo Total Potencia (USD)",
                                                "Costo Total Energía (USD)",
                                                "Costo Total (USD)"
            };
            #endregion

            //cargar hojas excel
            #region Hoja excel 

            int row = 1;
            int col = 1;

            for (var i = 0; i < labelsResult.Count(); i++)
            {
                ws.Cells[row, col].Value = labelsResult[i];
                col++;
            }
            row++;

            foreach (var item in list)
            {
                col = 1;
                foreach (var valor in (IEnumerable)item)
                {
                    ws.Cells[row, col].Value = valor.ToString();
                    col++;
                }
                row++;
            }
            #endregion

            using (MemoryStream oStream = new MemoryStream())
            {
                package.SaveAs(oStream);
                oStream.Position = 0;
                bytes = oStream.ToArray();
            }

        }
        var file = new FileContentResult(bytes, "application/octet-stream");
        file.FileDownloadName = baseDatos + "_TablaResultado_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx";
        return file;
    }

    public object GetExcel(string baseDatos)
    {
        byte[] bytes;
        using (var package = new ExcelPackage())
        {
            //buscar supuesto
            #region BuscarSupuesto

            var ws = package.Workbook.Worksheets.Add("Mod_adjudicados");
            var sup = "mod_adjudicados";
            //var list = Buscar(baseDatos, sup.Label.ToString());
            var list = Services.statelessSession.CreateSQLQuery(string.Format(@"
            SELECT *
            FROM `{0}`.{1};
            ", baseDatos, sup)).List<object>();


            var labelsResult = new string[] {   "Nombre",
                                                "AE",
                                                "Seleccionado",
                                                "PGMx",
                                                "PGMn",
                                                "PMedAdj",
                                                "Energia",
                                                "PorcentajeReduccion",
                                                "PorcentajeParaTotal",
                                                "MonomicoTot"
            };
            #endregion


            //cargar hojas excel
            #region Hoja excel 

            int row = 1;
            int col = 1;

            for (var i = 0; i < labelsResult.Count(); i++)
            {
                ws.Cells[row, col].Value = labelsResult[i];
                col++;
            }
            row++;

            foreach (var item in list)
            {
                col = 1;
                foreach (var valor in (IEnumerable)item)
                {
                    ws.Cells[row, col].Value = valor.ToString();
                    col++;
                }
                row++;
            }
            #endregion

            using (MemoryStream oStream = new MemoryStream())
            {
                package.SaveAs(oStream);
                oStream.Position = 0;
                bytes = oStream.ToArray();
            }

        }
        var file = new FileContentResult(bytes, "application/octet-stream");
        file.FileDownloadName = baseDatos + "_Mod_Adjudicados_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx";
        return file;
    }



    public IList<GraficoDto> ObtenerGraficos(string baseDatos)
    {
        var query = Services.statelessSession.CreateSQLQuery(string.Format(@"
            SELECT *
            FROM `{0}`.res_resultado
            ORDER BY case when ( ID_Oferente+0 = 0) 
			    then 
                    case when(ID_Oferente = 'Total Neto OV') then 1000 else 999 end
		        else ID_Oferente+0 
		        end;
            ", baseDatos));

        query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(GraficoDto)));
        return query.List<GraficoDto>();

    }

    public class GraficoDto
    {
        public string ID_Oferente { get; set; }
        public string Nombre { get; set; }
        public string Contrato { get; set; }
        public string ID_Comb1 { get; set; }
        public string ID_Comb2 { get; set; }
        public long Flexibilidad { get; set; }
        public double Precio_Potencia { get; set; }
        public double Precio_Energia { get; set; }
        public double Energia_Asignada { get; set; }
        public double Potencia_Media_Ofertada { get; set; }
        public double Potencia_Media_Adjudicada { get; set; }
        public long Periodos_de_Potencia_Adjudicada { get; set; }
        public long Periodos_de_Energia_Asignada { get; set; }
        public double Costo_Total_Potencia { get; set; }
        public double Costo_Total_Energia { get; set; }
        public double Costo_Total { get; set; }
    }
}
