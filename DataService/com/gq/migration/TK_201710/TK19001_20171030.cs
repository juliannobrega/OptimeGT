using FluentMigrator;
using System;

namespace MEMDataService.com.gq.migration
{
    [Migration(1900120171030, "Nuevas Graficos para Panama")]
    public class TK19001_20171030 : Migration
    {
        //es el 18333 pero este script debe correr ultimo
        public override void Up()
        {
            Delete.FromTable("Gq_tipos_grafico").AllRows();

            Insert.IntoTable("Gq_tipos_grafico").Row(new
            {
                Id = "1",
                Nombre = "Potencia"                
            });

            Insert.IntoTable("Gq_tipos_grafico").Row(new
            {
                Id = "2",
                Nombre = "Energía"
            });

            Insert.IntoTable("Gq_tipos_grafico").Row(new
            {
                Id = "3",
                Nombre = "Resultados"
            });

            Delete.FromTable("gq_grafico").AllRows();
            Alter.Table("gq_grafico").AlterColumn("TipoSeleccion").AsInt32().Nullable();

            Insert.IntoTable("Gq_grafico").Row(new
            {
                Nombre = "Potencia Total por Tipo de Contrato",
                Descripcion = "Potencia Total por Tipo de Contrato",
                Folder = "PotenciaTotalporTipo",
                Template = "",
                Scritp = "",
                CodeSharp = "",
                Estado = "A",
                Creado = DateTime.Now,
                CreadoPor = 1,
                Modificado = DateTime.Now,
                ModificadoPor = 1,
                TipoId = 1
            });

            Insert.IntoTable("Gq_grafico").Row(new
            {
                Nombre = "Potencia por Oferente y contrato",
                Descripcion = "Potencia por Oferente y contrato",
                Folder = "PotenciaporOferenteyContrato",
                Template = "",
                Scritp = "",
                CodeSharp = "",
                Estado = "A",
                Creado = DateTime.Now,
                CreadoPor = 1,
                Modificado = DateTime.Now,
                ModificadoPor = 1,
                TipoId = 1
            });

            Insert.IntoTable("Gq_grafico").Row(new
            {
                Nombre = "Energía Total por Tipo de Contrato",
                Descripcion = "Energía Total por Tipo de Contrato",
                Folder = "EnergiaTotalporTipodeContrato",
                Template = "",
                Scritp = "",
                CodeSharp = "",
                Estado = "A",
                Creado = DateTime.Now,
                CreadoPor = 1,
                Modificado = DateTime.Now,
                ModificadoPor = 1,
                TipoId = 2
            });

            Insert.IntoTable("Gq_grafico").Row(new
            {
                Nombre = "Energía por Oferente y contrato",
                Descripcion = "Energía por Oferente y contrato",
                Folder = "EnergiaporOferenteyContrato",
                Template = "",
                Scritp = "",
                CodeSharp = "",
                Estado = "A",
                Creado = DateTime.Now,
                CreadoPor = 1,
                Modificado = DateTime.Now,
                ModificadoPor = 1,
                TipoId = 2
            });

            Insert.IntoTable("Gq_grafico").Row(new
            {
                Nombre = "Energía Asociada a la Potencia",
                Descripcion = "Energía Asociada a la Potencia",
                Folder = "EnergiaAsociadaalaPotencia",
                Template = "",
                Scritp = "",
                CodeSharp = "",
                Estado = "A",
                Creado = DateTime.Now,
                CreadoPor = 1,
                Modificado = DateTime.Now,
                ModificadoPor = 1,
                TipoId = 2
            });

            Insert.IntoTable("Gq_grafico").Row(new
            {
                Nombre = "Tabla Resultado",
                Descripcion = "Tabla Resultado",
                Folder = "TablaResultado",
                Template = "",
                Scritp = "",
                CodeSharp = "",
                Estado = "A",
                Creado = DateTime.Now,
                CreadoPor = 1,
                Modificado = DateTime.Now,
                ModificadoPor = 1,
                TipoId = 3
            });

        }

        public override void Down()
        {
        }
    }
}
