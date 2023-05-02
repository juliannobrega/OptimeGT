using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GQService.com.gq.excel
{
    /// <summary>
    /// 
    /// </summary>
    public class Worksheet
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        public Worksheet(string Name)
        {
            this.Name = Name;
            Rows = new Dictionary<int, Row>();
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<int, Row> Rows { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="cell"></param>
        /// <returns></returns>
        public Cell AddCell(int row, int col, Cell cell)
        {
            Row r;
            if (!Rows.ContainsKey(row))
            {
                r = new Row();
                Rows.Add(row, r);
            }
            else
            {
                r = Rows[row];
            }

            r.AddCell(col, cell);

            return cell;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        public void GenerateByIEnumerable(IEnumerable list)
        {
            Type type = list.GetType();
            Type subType = null;
            int row = 1;
            int col = 1;
            if (type.GenericTypeArguments.Count() > 0)
            {
                subType = type.GenericTypeArguments[0];
                PropertyInfo[] props = subType.GetProperties();
                col = 1;
                for (var i = 0; i < props.Length; i++)
                {
                    if (props[i].CanRead && props[i].CanWrite)
                    {
                        AddCell(row, col, new Cell(props[i].Name));
                        col++;
                    }
                }
                Rows[row].StyleID = "Header";
                row++;

                foreach (var item in list)
                {
                    col = 1;
                    for (var i = 0; i < props.Length; i++)
                    {
                        if (props[i].CanRead && props[i].CanWrite)
                        {
                            if (props[i].GetValue(item) != null)
                                AddCell(row, col, new Cell(props[i].GetValue(item).ToString()));
                            else
                                AddCell(row, col, new Cell());

                            col++;
                        }
                    }
                    row++;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        public void GenerateByIEnumerableDictionary(IEnumerable<IDictionary> list)
        {
            int row = 1;
            int col = 1;

            foreach (var item in list)
            {
                if (row==1)
                {
                    col = 1;
                    foreach (var key in item.Keys)
                    {
                        AddCell(row, col, new Cell(key));
                        col++;
                    }
                    Rows[row].StyleID = "Header";
                    row++;
                }


                col = 1;
                foreach (var value in item.Values)
                {
                    if (value != null)
                        AddCell(row, col, new Cell(value.ToString()));
                    else
                        AddCell(row, col, new Cell());
                    col++;
                }
                row++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        public void GenerateByIEnumerableDynamicTable(IList<object> list, int longitud)
        {
            int row = 1;
            int col = 1;

            foreach (IList<object> item in list)
            {
                col = 1;
                for (var i = 0; i < item.Count; i++)
                {
                    if (item[i] == null)
                        AddCell(row, col, new Cell(""));
                    else
                        AddCell(row, col, new Cell(item[i].ToString()));
                    col++;
                }
                if (row == 1)
                    Rows[row].StyleID = "Header";
                row++;
            }
        }

    }
}
