using GQService.com.gq.dto;
using GQService.com.gq.utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;

namespace GQService.com.gq.paging
{
    /// <summary>
    /// Permite paginar una busqueda de la base de datos
    /// </summary>
    public class Paging
    {
        /// <summary>
        /// Pagina que va a leer valor minimo 1
        /// </summary>
        public virtual int? PageIndex { get; set; }
        /// <summary>
        /// Tamaño de la pagina valor minimo 1
        /// </summary>
        public virtual int? PageSize { get; set; }
        /// <summary>
        /// Total de paginas
        /// </summary>
        public virtual int? PageCount { get; set; }
        /// <summary>
        /// Catidad de registros
        /// </summary>
        public virtual int? RecordCount { get; set; }
        /// <summary>
        /// Filtro de los resultados
        /// </summary>
        public List<PagingFilter> Filter { get; set; } = new List<PagingFilter>();
        /// <summary>
        /// Orden de los resultados
        /// </summary>
        public List<PagingOrder> Order { get; set; } = new List<PagingOrder>();
        /// <summary>
        /// Datos que va a mostrar
        /// </summary>
        public virtual ArrayList data { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Paging()
        {
            PageIndex = 1;
            PageSize = 25;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        public Paging(int PageIndex, int PageSize)
        {
            this.PageIndex = PageIndex;
            this.PageSize = PageSize;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">Entidad</typeparam>
        /// <typeparam name="E">Entidad IDto</typeparam>
        /// <param name="source">Consulta Linq</param>
        public void Apply<T, E>(IQueryable<T> source)
            where T : IEntity, new()
            where E : IDto<T, E>, new()
        {

            if (Filter != null)
            {
                //Aplicar Filtro
                if (Filter.Count > 0)
                {
                    foreach (var item in Filter)
                    {
                        if (item.Condition != null)
                        {
                            switch (item.Condition.ToString())
                            {
                                case "in":
                                    {
                                        source = source.Where(item.Property + " in (@0)", item.Value.ToString());
                                        break;
                                    }
                                case "inArray":
                                    {
                                        var array = ((List<long>)item.Value).ToList();
                                        source = source.Where("@0.Contains(" + item.Property + ")", array);
                                        break;
                                    }
                                case "con":
                                    {
                                        source = source.Where(item.Property + ".Contains(@0)", item.Value.ToString());
                                        break;
                                    }
                                case "x":
                                    {
                                        break;
                                    }
                                case "=|T":
                                    {
                                        if (item.Value.ToString() != "T") source = source.Where(item.Property + " = @0 ", item.Value);
                                        break;
                                    }
                                default:
                                    {
                                        source = source.Where(item.Property + " " + item.Condition.ToString() + " @0 ", item.Value);
                                        break;
                                    }
                            }

                        }
                    }
                }
                int total = source.Count();
                this.RecordCount = total;
                this.PageCount = total / PageSize;

                if (total % PageSize > 0)  PageCount++;
            }

            if (Order != null)
            {
                ////Aplicar Orden
                if (Order.Count > 0)
                {
                    string orderBy = "";
                    foreach (var item in Order)
                    {
                        orderBy = orderBy + item.Property + " " + (item.Direction == "+" ? "asc" : "desc") + ",";
                    }
                    orderBy = orderBy.Substring(0, orderBy.Length - 1);
                    source = source.OrderBy(orderBy);
                }
            }


            List<T> SourceData = source.Skip((PageIndex.Value - 1) * PageSize.Value).Take(PageSize.Value).ToList();


            data = new ArrayList();

            E items = new E();

            sourceData = items.makeDto(SourceData);

            data.AddRange((ICollection)sourceData);
        }

        IList sourceData = null;
        public IList GetSourceData()
        {
            return sourceData;
        }

        public class PagingFilter
        {
            public string Property { get; set; }
            public string Condition { get; set; }
            public object Value { get; set; }
        }

        public class PagingOrder
        {
            public string Property { get; set; }
            public string Direction { get; set; }
        }
    }
}
