using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace GQService.com.gq.excel
{
    /// <summary>
    /// 
    /// </summary>
    public static class ExcelEncode
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="wb"></param>
        /// <returns></returns>
        public static XDocument Encode(Workbook wb)
        {
            XNamespace mainNamespace = "urn:schemas-microsoft-com:office:spreadsheet";
            XNamespace o = "urn:schemas-microsoft-com:office:office";
            XNamespace x = "urn:schemas-microsoft-com:office:excel";
            XNamespace ss = "urn:schemas-microsoft-com:office:spreadsheet";
            XNamespace html = "http://www.w3.org/TR/REC-html40";

            XDocument xdoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
            XElement XElementStyle;

            if (wb.Styles.Count > 0)
            {
                XElementStyle = new XElement(mainNamespace + "Styles",
                    from contentRow in wb.Styles
                    select new XElement(mainNamespace + "Style",
                            new XAttribute(ss + "ID", contentRow.Id),
                            new XAttribute(ss + "Name", contentRow.Name),
                            from e in contentRow.Elements
                            select new XElement(mainNamespace + e.Name,
                                    from a in e.Attribute
                                    select new XAttribute(ss + a.Name, a.Value)
                               )));
            }
            else
            {
                XElementStyle = new XElement(mainNamespace + "Styles",
                           new XElement(mainNamespace + "Style",
                               new XAttribute(ss + "ID", "Default"),
                               new XAttribute(ss + "Name", "Normal"),
                               new XElement(mainNamespace + "Alignment",
                                   new XAttribute(ss + "Vertical", "Bottom")
                               ),
                               new XElement(mainNamespace + "Borders"),
                               new XElement(mainNamespace + "Font",
                                   new XAttribute(ss + "FontName", "Calibri"),
                                   new XAttribute(x + "Family", "Swiss"),
                                   new XAttribute(ss + "Size", "11"),
                                   new XAttribute(ss + "Color", "#000000")
                               ),
                               new XElement(mainNamespace + "Interior"),
                               new XElement(mainNamespace + "NumberFormat"),
                               new XElement(mainNamespace + "Protection")
                           ),
                           new XElement(mainNamespace + "Style",
                               new XAttribute(ss + "ID", "Header"),
                               new XElement(mainNamespace + "Font",
                                   new XAttribute(ss + "FontName", "Calibri"),
                                   new XAttribute(x + "Family", "Swiss"),
                                   new XAttribute(ss + "Size", "11"),
                                   new XAttribute(ss + "Color", "#000000"),
                                   new XAttribute(ss + "Bold", "1")
                               )
                           )
                       );
            }

            List<XElement> worksheets = new List<XElement>();

            for (var i = 0; i < wb.Worksheets.Count; i++)
            {
                int maxRow = -1;
                int maxCol = -1;
                List<XElement> rows = new List<XElement>();

                foreach (int row in wb.Worksheets[i].Rows.Keys)
                {
                    maxRow = Math.Max(maxRow, row + 1);
                    List<XElement> cols = new List<XElement>();
                    foreach (int col in wb.Worksheets[i].Rows[row].Cells.Keys)
                    {
                        maxCol = Math.Max(maxCol, col + 1);
                        cols.Add(new XElement(mainNamespace + "Cell", new XAttribute(ss + "Index", col), (string.IsNullOrWhiteSpace(wb.Worksheets[i].Rows[row].Cells[col].Formula) ? null : new XAttribute(ss + "Formula", wb.Worksheets[i].Rows[row].Cells[col].Formula)),
                                new XElement(mainNamespace + "Data",
                                    new XAttribute(ss + "Type", wb.Worksheets[i].Rows[row].Cells[col].Type.ToString()), wb.Worksheets[i].Rows[row].Cells[col].Data)));
                    }
                    rows.Add(new XElement(mainNamespace + "Row", new XAttribute(ss + "Index", row), new XAttribute(ss + "StyleID", wb.Worksheets[i].Rows[row].StyleID), from c in cols select c));
                }
                //                            new XElement(mainNamespace + "Column", new XAttribute(ss + "Width", 81)),
                worksheets.Add(new XElement(mainNamespace + "Worksheet",
                        new XAttribute(ss + "Name", wb.Worksheets[i].Name /* Sheet name */),
                        new XElement(mainNamespace + "Table",
                            new XAttribute(ss + "ExpandedColumnCount", maxCol),
                            new XAttribute(ss + "ExpandedRowCount", maxRow + 4),
                            new XAttribute(x + "FullColumns", 1),
                            new XAttribute(x + "FullRows", 1),
                            new XAttribute(ss + "DefaultRowHeight", 15),
                            from r in rows select r),
                         new XElement(x + "WorksheetOptions",
                            new XAttribute(XName.Get("xmlns", ""), x),
                            /* new XElement(x + "PageSetup",
                                 new XElement(x + "Header",
                                     new XAttribute(x + "Margin", "0.3")
                                 ),
                                 new XElement(x + "Footer",
                                     new XAttribute(x + "Margin", "0.3")
                                 ),
                                 new XElement(x + "PageMargins",
                                     new XAttribute(x + "Bottom", "0.75"),
                                     new XAttribute(x + "Left", "0.7"),
                                     new XAttribute(x + "Right", "0.7"),
                                     new XAttribute(x + "Top", "0.75")
                                 )
                             ),*/
                            new XElement(x + "Print",
                                new XElement(x + "ValidPrinterInfo"),
                                new XElement(x + "HorizontalResolution", 600),
                                new XElement(x + "VerticalResolution", 600)
                            ),
                            new XElement(x + "Selected"),
                            new XElement(x + "Panes",
                                new XElement(x + "Pane",
                                    new XElement(x + "Number", 3),
                                    new XElement(x + "ActiveRow", 5),
                                    new XElement(x + "ActiveCol", 1)
                                )
                            ),
                            new XElement(x + "ProtectObjects", "False"),
                            new XElement(x + "ProtectScenarios", "False")
                        ) // close worksheet options
                    ));
            }

            XElement workbook = new XElement(mainNamespace + "Workbook",
                new XAttribute(XNamespace.Xmlns + "html", html),
                new XAttribute(XName.Get("ss", "http://www.w3.org/2000/xmlns/"), ss),
                new XAttribute(XName.Get("o", "http://www.w3.org/2000/xmlns/"), o),
                new XAttribute(XName.Get("x", "http://www.w3.org/2000/xmlns/"), x),
                new XAttribute(XName.Get("xmlns", ""), mainNamespace),
                new XElement(o + "DocumentProperties",
                        new XAttribute(XName.Get("xmlns", ""), o),
                        new XElement(o + "Author", wb.DocumentProperties.Author),
                        new XElement(o + "LastAuthor", wb.DocumentProperties.LastAuthor),
                        new XElement(o + "Created", wb.DocumentProperties.Created),
                        new XElement(o + "Company", wb.DocumentProperties.Created),
                        new XElement(o + "Version", wb.DocumentProperties.Created)

                    ), //end document properties
                new XElement(x + "ExcelWorkbook",
                        new XAttribute(XName.Get("xmlns", ""), x),
                        new XElement(x + "WindowHeight", 12750),
                        new XElement(x + "WindowWidth", 24855),
                        new XElement(x + "WindowTopX", 240),
                        new XElement(x + "WindowTopY", 75),
                        new XElement(x + "ProtectStructure", "False"),
                        new XElement(x + "ProtectWindows", "False")
                    ), //end ExcelWorkbook
                XElementStyle
               , // close styles
                from sheet in worksheets
                select sheet
                );

            xdoc.Add(workbook);

            return xdoc;
        }
    }
}
