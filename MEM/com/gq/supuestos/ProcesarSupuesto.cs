using GQService.com.gq.compiler;
using GQService.com.gq.service;
using MEMDataService.com.gq.service;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace MEM.com.gq.supuestos
{
    public static class ProcesarSupuesto
    {
        public static object Ejecutar(EjecutarDto model)
        {
            object result = null;
            var g = Services.Get<ServGq_supuesto>().findById(model.Id);
            if (g != null)
            {
                CompilerCSharp cs = new CompilerCSharp();

                cs.AddReferencia("System.dll");
                cs.AddReferencia("System.Data.dll");
                cs.AddReferencia("System.Core.dll");
                cs.AddReferencia("System.Runtime.dll");
                cs.AddReferencia("System.Runtime.Serialization.dll");

                var files = System.IO.Directory.GetFiles(cs.PathBase, "*.dll");
                var excludeDlls = Startup.Configuration.GetSection("ExcludeLibs").Get<string[]>();

                foreach (var item in files)
                {
                    var fileName = item.Substring(item.LastIndexOf('\\') + 1).ToLower();
                    if (excludeDlls.Where(x => x.Equals(fileName)).Count() == 0)
                        cs.AddReferencia(item);
                }

                cs.AddReferencia(cs.PathBase + "MEM.exe");

                if (!string.IsNullOrWhiteSpace(g.Folder))
                {
                    var dir = System.IO.Directory.GetCurrentDirectory();
                    cs.SourceType = CompilerCSharp.SourceTypeEnum.File;
                    cs.Source = dir + "\\wwwroot\\supuestos\\" + g.Folder + "\\supuesto.cs";
                }
                else
                {
                    cs.SourceType = CompilerCSharp.SourceTypeEnum.Text;
                    cs.Source = g.CodeSharp;
                }

                result = cs.Invoke("Main", model.Metodo, model.Parametros);

            }
            return result;
        }

        public class EjecutarDto
        {
            public long Id { get; set; }
            public string Metodo { get; set; }
            public object[] Parametros { get; set; }
        }


    }
}