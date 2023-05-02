using GQService.com.gq.data;
using GQService.com.gq.dto;
using GQService.com.gq.log;
using GQService.com.gq.service;
using MEM.com.gq.supuestos;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class Main
{
    private int fila = 0;
    private int columna = 0;
    private string hoja = string.Empty;
    private bool errorUpdate = false;

    public IList<ComboBoxDto> BuscarListaSupuesto(string baseDatos)
    {
        var query = Services.session.CreateSQLQuery(@"SELECT Id , TablaMod AS Label
                                                    FROM gq_supuesto
                                                    WHERE Folder != 'sup_ImpExpTodos'");
        query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(ComboBoxDto)));
        return query.List<ComboBoxDto>();
    }

    public IList<ComboBoxDto> BuscarListaMod(string baseDatos)
    {
        var query = Services.session.CreateSQLQuery(@"SELECT Id , TablaMod AS Label
                                                    FROM gq_supuesto
                                                    WHERE Folder != 'sup_ImpExpTodos' 
                                                    ORDER BY Nombre");
        query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(ComboBoxDto)));
        return query.List<ComboBoxDto>();
    }

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

    public object GetExcel(string baseDatos, bool selectdIsSup = true)
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

    public ReturnData ImportarExcel(string schema, string excelFilename)
    {
        var result = new ReturnData();
        SupuestoInfo sup = new SupuestoInfo();
        
        try
        {
            ExcelPackage ep = new ExcelPackage();
            ep.Load(File.Open(excelFilename, FileMode.Open));
          
            Task.Factory.StartNew(async () =>
            {
                for (int i = 1; i <= ep.Workbook.Worksheets.Count; i++)
                {
                    hoja = ep.Workbook.Worksheets[i].Name;
                    var labelsResult = Services.statelessSession.CreateSQLQuery(string.Format(@"
                    SELECT `COLUMN_NAME` , `DATA_TYPE`
                    FROM `INFORMATION_SCHEMA`.`COLUMNS` 
                    WHERE `TABLE_SCHEMA`= '{0}'
                    AND `TABLE_NAME`='{1}'
                    ORDER BY `ORDINAL_POSITION`
                    ", schema, ep.Workbook.Worksheets[i].Name))
                            .SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(SchemaInformation))).List<SchemaInformation>();
                    sup.Fin = false;
                    sup.Nombre = ep.Workbook.Worksheets[i].Name;
                    sup.Total = ep.Workbook.Worksheets[i].Dimension.Rows;
                    if (ep.Workbook.Worksheets[i].Dimension.Rows > 0 && labelsResult.Count > 0)
                    {
                        using (var transaction = Services.statelessSession.BeginTransaction())
                        {
                            var query = Services.statelessSession.CreateSQLQuery(string.Format(@"DELETE FROM `{0}`.{1} ", schema, ep.Workbook.Worksheets[i].Name));
                            query.ExecuteUpdate();

                            var label0 = string.Empty;
                            var label1 = string.Empty;
                            var sqlerror = string.Empty;

                            try
                            {
                                int row = 2;

                                while (row < ep.Workbook.Worksheets[i].Dimension.Rows)
                                {
                                    var sql = string.Format(@"INSERT INTO `{0}`.`{1}` (", schema, ep.Workbook.Worksheets[i].Name);

                                    for (int j = 0; j < labelsResult.Count; j++)
                                    {
                                        sql += "`" + labelsResult[j].COLUMN_NAME + "`";
                                        if (j < labelsResult.Count - 1)
                                        {
                                            sql += ",";
                                        }
                                    }
                                    sql += " ) ";


                                    if (ep.Workbook.Worksheets[i].Dimension.Rows > 1)
                                    {
                                        sql += " VALUES ";
                                        int limite = row + 1000;
                                        for (var z = row; z <= limite; z++)
                                        {
                                            row = z;
                                            sup.Index = z;

                                            //fila = z;
                                            //columna = 0;

                                            sql += "(";
                                            
                                            for (int j = 0; j < labelsResult.Count; j++)
                                            {
                                                columna++;

                                                if  (ep.Workbook.Worksheets[i].Cells[z, j + 1].Value == null)
                                                {
                                                    sql += "'" + "'";
                                                } else
                                                {
                                                    sql += "'" + ep.Workbook.Worksheets[i].Cells[z, j + 1].Value.ToString().Replace(',', '.') + "'";
                                                }
                                                if (j < labelsResult.Count - 1)
                                                {
                                                    sql += ", ";
                                                }
                                            }
                                            

                                            //query.ExecuteUpdate();

                                            if ((z + 1 == limite) || (ep.Workbook.Worksheets[i].Cells[z + 1, 1].Value == null))
                                            {
                                                row++;
                                                sql += ")";
                                                break;
                                            }
                                            else
                                            {
                                                sql += "),";
                                            }
                                        }
                                        sqlerror = sql;
                                        query = Services.statelessSession.CreateSQLQuery(sql);
                                        query.ExecuteUpdate();

                                        //Services.statelessSession.Dispose();

                                    }
                                }

                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                Log.Error("ImportarExcel ", ex);
                                sup.Fin = true;
                                sup.Error = true;
                                if (errorUpdate)
                                    sup.ErrorMsj = "Se produjo un error en la carga del archivo. Hoja: '" + hoja + ", Fila: " + sup.Index;
                                else
                                    sup.ErrorMsj = "Error en la Hoja: '" + hoja + ", Fila: " + sup.Index ;

                                break;
                            }
                        }
                    }

                }
                sup.Fin = true;
            });

            result.data = sup;
        }
        catch (Exception ex)
        {
            sup.Fin = true;
            sup.Error = true;
            sup.ErrorMsj = "Se produjo un error en la carga del archivo.";

            Log.Error("ImportarExcel ", ex);
            result.isError = true;
            result.data = "Se produjo un error en la carga del archivo.";
            return result;
        }
        return result;
    }
}
