using Ionic.Zip;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GQService.com.gq.excel
{
    /// <summary>
    /// 
    /// </summary>
    public static class XLSXEncode
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="wb"></param>
        /// <param name="stream"></param>
        public static void Encode(Workbook wb, Stream stream)
        {
            var sb = new StringBuilder();
            using (ZipFile zip = new ZipFile())
            {
                zip.AddEntry("[Content_Types].xml", @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>
<Types xmlns=""http://schemas.openxmlformats.org/package/2006/content-types""><Default ContentType=""application/xml"" Extension=""xml""/><Default ContentType=""application/vnd.openxmlformats-package.relationships+xml"" Extension=""rels""/><Override ContentType=""application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml"" PartName=""/xl/worksheets/sheet1.xml""/><Override ContentType=""application/vnd.openxmlformats-officedocument.spreadsheetml.sharedStrings+xml"" PartName=""/xl/sharedStrings.xml""/><Override ContentType=""application/vnd.openxmlformats-officedocument.drawing+xml"" PartName=""/xl/drawings/worksheetdrawing1.xml""/><Override ContentType=""application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml"" PartName=""/xl/styles.xml""/><Override ContentType=""application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml"" PartName=""/xl/workbook.xml""/></Types>");

                zip.AddEntry(@"_rels\.rels", @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>
<Relationships xmlns=""http://schemas.openxmlformats.org/package/2006/relationships""><Relationship Id=""rId1"" Type=""http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument"" Target=""xl/workbook.xml""/></Relationships>");

                var workbookSB = new StringBuilder();
                workbookSB.Append(@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>
<Relationships xmlns=""http://schemas.openxmlformats.org/package/2006/relationships""><Relationship Id=""rId1"" Type=""http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles"" Target=""styles.xml""/><Relationship Id=""rId2"" Type=""http://schemas.openxmlformats.org/officeDocument/2006/relationships/sharedStrings"" Target=""sharedStrings.xml""/>");
                foreach (var ws in wb.Worksheets)
                {
                    workbookSB.Append(@"<Relationship Id=""rId" + (wb.Worksheets.IndexOf(ws) + 3) + @""" Type=""http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet"" Target=""worksheets/sheet" + (wb.Worksheets.IndexOf(ws) + 1) + @".xml""/>");
                }
                workbookSB.Append(@"</Relationships>");
                zip.AddEntry(@"xl\_rels\workbook.xml.rels", workbookSB.ToString());

                zip.AddEntry(@"xl\drawings\worksheetdrawing1.xml", @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>
<xdr:wsDr xmlns:xdr=""http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing"" xmlns:a=""http://schemas.openxmlformats.org/drawingml/2006/main"" xmlns:r=""http://schemas.openxmlformats.org/officeDocument/2006/relationships"" xmlns:c=""http://schemas.openxmlformats.org/drawingml/2006/chart"" xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006"" xmlns:dgm=""http://schemas.openxmlformats.org/drawingml/2006/diagram""/>");

                zip.AddEntry(@"xl\styles.xml", @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>
<styleSheet xmlns=""http://schemas.openxmlformats.org/spreadsheetml/2006/main"" xmlns:x14ac=""http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac"" xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""><fonts count=""2""><font><sz val=""10.0""/><color rgb=""FF000000""/><name val=""Arial""/></font><font/></fonts><fills count=""2""><fill><patternFill patternType=""none""/></fill><fill><patternFill patternType=""lightGray""/></fill></fills><borders count=""1""><border><left/><right/><top/><bottom/></border></borders><cellStyleXfs count=""1""><xf borderId=""0"" fillId=""0"" fontId=""0"" numFmtId=""0"" applyAlignment=""1"" applyFont=""1""/></cellStyleXfs><cellXfs count=""2""><xf borderId=""0"" fillId=""0"" fontId=""0"" numFmtId=""0"" xfId=""0"" applyAlignment=""1"" applyFont=""1""><alignment/></xf><xf borderId=""0"" fillId=""0"" fontId=""1"" numFmtId=""0"" xfId=""0"" applyAlignment=""1"" applyFont=""1""><alignment/></xf></cellXfs><cellStyles count=""1""><cellStyle xfId=""0"" name=""Normal"" builtinId=""0""/></cellStyles><dxfs count=""0""/></styleSheet>");

                var sheetsSB = new StringBuilder();
                sheetsSB.Append(@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>
<workbook xmlns=""http://schemas.openxmlformats.org/spreadsheetml/2006/main"" xmlns:r=""http://schemas.openxmlformats.org/officeDocument/2006/relationships"" xmlns:mx=""http://schemas.microsoft.com/office/mac/excel/2008/main"" xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006"" xmlns:mv=""urn:schemas-microsoft-com:mac:vml"" xmlns:x14=""http://schemas.microsoft.com/office/spreadsheetml/2009/9/main"" xmlns:x14ac=""http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac"" xmlns:xm=""http://schemas.microsoft.com/office/excel/2006/main""><workbookPr/><sheets>");
                foreach (var ws in wb.Worksheets)
                {
                    sheetsSB.Append(@"<sheet state=""visible"" name=""" + ws.Name + @""" sheetId=""" + (wb.Worksheets.IndexOf(ws) + 1) + @""" r:id=""rId"+ (wb.Worksheets.IndexOf(ws) + 3) + @"""/>");
                }
                sheetsSB.Append(@"</sheets><definedNames/><calcPr/></workbook>");
                zip.AddEntry(@"xl\workbook.xml", sheetsSB.ToString());

                List<string> tIndex = new List<string>();
                foreach (var ws in wb.Worksheets)
                {
                    sb = new StringBuilder();

                    sb.Append(@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>");
                    sb.Append(@"<worksheet xmlns=""http://schemas.openxmlformats.org/spreadsheetml/2006/main"" xmlns:r=""http://schemas.openxmlformats.org/officeDocument/2006/relationships"" xmlns:mx=""http://schemas.microsoft.com/office/mac/excel/2008/main"" xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006"" xmlns:mv=""urn:schemas-microsoft-com:mac:vml"" xmlns:x14=""http://schemas.microsoft.com/office/spreadsheetml/2009/9/main"" xmlns:x14ac=""http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac"" xmlns:xm=""http://schemas.microsoft.com/office/excel/2006/main""><sheetViews><sheetView workbookViewId=""0""/></sheetViews><sheetFormatPr customHeight=""1"" defaultColWidth=""14.43"" defaultRowHeight=""15.75""/><sheetData>");

                    foreach (var row in ws.Rows)
                    {
                        sb.Append(@"<row r=""" + row.Key + @""">");
                        //Log.Debug("XLSXEncode.Encode 7.2 ROW = " + row.Key + " COLS = " + row.Value.Cells.Count);
                        foreach (var cell in row.Value.Cells)
                        {
                            if (cell.Value.Type == Cell.CellType.String)
                            {
                                sb.Append(@"<c r=""" + Cell.GetExcelColumnName(cell.Key) + row.Key + @""" s=""1"" t=""s""><v>" + tIndex.Count + "</v></c>");
                                tIndex.Add(cell.Value.Data);
                            }
                            else if (cell.Value.Type == Cell.CellType.Number)
                            {
                                sb.Append(@"<c r=""" + Cell.GetExcelColumnName(cell.Key) + row.Key + @""" s=""1""><v>" + cell.Value.Data + "</v></c>");
                            }
                            else if (cell.Value.Type == Cell.CellType.Function)
                            {
                                sb.Append(@"<c r=""" + Cell.GetExcelColumnName(cell.Key) + row.Key + @""" s=""1""><f>" + cell.Value.Data + "</f></c>");
                            }
                        }
                        sb.Append(@"</row>");
                    }
                    sb.Append(@"</sheetData><drawing r:id=""rId1""/></worksheet>");

                    zip.AddEntry(@"xl\worksheets\_rels\sheet" + (wb.Worksheets.IndexOf(ws) + 1) + @".xml.rels", @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>
<Relationships xmlns=""http://schemas.openxmlformats.org/package/2006/relationships""><Relationship Id=""rId1"" Type=""http://schemas.openxmlformats.org/officeDocument/2006/relationships/drawing"" Target=""../drawings/worksheetdrawing1.xml""/></Relationships>");

                    zip.AddEntry(@"xl\worksheets\sheet" + (wb.Worksheets.IndexOf(ws) + 1) + @".xml", sb.ToString());
                }

                sb = new StringBuilder();
                sb.Append(@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>");
                sb.Append(@"<sst xmlns=""http://schemas.openxmlformats.org/spreadsheetml/2006/main"" count=""" + tIndex.Count + @""" uniqueCount=""" + tIndex.Count + @""" >");
                //Log.Debug("XLSXEncode.Encode 10");
                foreach (var s in tIndex)
                {
                    sb.Append(@"<si><t>" + Encode(s) + @"</t></si>");
                }
                sb.Append(@"</sst>");
                //Log.Debug("XLSXEncode.Encode 11");
                zip.AddEntry(@"xl\sharedStrings.xml", sb.ToString());
                //Log.Debug("XLSXEncode.Encode 12");
                zip.Save(stream);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Encode(string text)
        {
            byte[] myASCIIBytes = ASCIIEncoding.ASCII.GetBytes(text);
            byte[] myUTF8Bytes = ASCIIEncoding.Convert(ASCIIEncoding.ASCII, UTF8Encoding.UTF8, myASCIIBytes);
            return UTF8Encoding.UTF8.GetString(myUTF8Bytes);
        }
    }
}