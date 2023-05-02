using GQService.com.gq.service;
using GQService.com.gq.utils;
using MEMDataService.com.gq.domain;
using MEMDataService.com.gq.InterfaceLibreriaExterna;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;

namespace MEM.com.gq.plugins
{
    public static class PluginsLoader
    {
        public class LogDataBase
        {
            public DateTime fecha { get; set; }
            public long orden { get; set; }
            public string tipo { get; set; }
            public string informacion { get; set; }
            public string plugin { get; set; }
            public string version { get; set; }
        }

        public static IEnumerable GetLog(string dataBase)
        {
            var sql = Services.session.CreateSQLQuery(string.Format(@"
            SELECT *
            FROM `{0}`.sis_log
            ORDER BY Fecha desc, Orden desc limit 100;
            ", dataBase));

            sql.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(LogDataBase)));
            return sql.List<LogDataBase>();
        }

        internal class AssemblyResolver : MarshalByRefObject, IPlugin
        {
            public string PathFolder;
            public string PathBase;
            public IPlugin Value;

            private List<Assembly> AssemblyList = new List<Assembly>();

            internal static AssemblyResolver Register(AppDomain domain, string PathFolder, string PathBase)
            {

                try
                {
                AssemblyResolver resolver =
                    domain.CreateInstanceFromAndUnwrap(
                      Assembly.GetExecutingAssembly().Location,
                      typeof(AssemblyResolver).FullName) as AssemblyResolver;

                resolver.PathBase = PathBase;
                resolver.PathFolder = PathFolder;

                resolver.RegisterDomain(domain);

                return resolver;
            }
                catch (Exception ex)
                {
                    return null;
                }
            }

            private void RegisterDomain(AppDomain domain)
            {
                domain.AssemblyResolve += ResolveAssembly;
            }

            public void LoadAssembly(string AssemblyPath)
            {
                try
                {
                    AssemblyList.Add(Assembly.LoadFrom(AssemblyPath));
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(ex.Message, ex);
                }
            }

            public void LoadPlugIn()
            {
                Value = (IPlugin)ClassUtils.getNewInstance(GetTypeBy());
            }

            private Type GetTypeBy()
            {

                var a = AssemblyList.Where(x => x.GetTypes().Where(j => j.GetInterfaces().Where(y => y.Name == "IPlugin").Count() > 0).FirstOrDefault() != null);

                var result = a.Select(x => x.GetTypes().Where(j => j.GetInterfaces().Where(y => y.Name == "IPlugin").Count() > 0).FirstOrDefault()).FirstOrDefault();

                return result;
            }

            private Assembly ResolveAssembly(object sender, ResolveEventArgs args)
            {
                FileInfo dll = null;
                var otherCompanyDlls = new DirectoryInfo(PathFolder).GetFiles("*.dll");
                var files = args.Name.Split(',');
                foreach (var file in files)
                {
                    var f = file.Split('\\');
                    if (f[f.Length - 1] == "MEM")
                        break;
                    else
                        dll = otherCompanyDlls.FirstOrDefault(fi => fi.Name.ToLower().Contains(f[f.Length - 1].ToLower()));

                    if (dll != null)
                        break;
                }

                if (dll == null)
                {
                    otherCompanyDlls = new DirectoryInfo(PathBase).GetFiles("*.dll");
                    foreach (var file in files)
                    {
                        var f = file.Split('\\');
                        if (f[f.Length - 1] == "MEM")
                        {
                            otherCompanyDlls = new DirectoryInfo(PathBase).GetFiles("*.exe");
                            dll = otherCompanyDlls.FirstOrDefault(fi => fi.Name.ToLower().Contains("MEM.EXE".ToLower()));
                        }
                        else
                            dll = otherCompanyDlls.FirstOrDefault(fi => fi.Name.ToLower().Contains(f[f.Length - 1].ToLower()));

                        if (dll != null)
                            break;
                    }
                }

                if (dll == null)
                    return null;

                return Assembly.LoadFrom(dll.FullName);
            }

            public string[] CommandsList()
            {
                return Value.CommandsList();
            }

            public string Command(string comando, params object[] parametros)
            {
                return Value.Command(comando, parametros);
            }

            public string Version()
            {
                return Value.Version();
            }
        }

        public class Plugin : IDisposable
        {
            public IPlugin Value { get; private set; }
            public string PathFolder { get; private set; }
            private Gq_plugin gqplugin;
            private AppDomain domain = null;

            public Plugin(Gq_plugin gqplugin)
            {
                string path = Assembly.GetExecutingAssembly().Location;
                var idx = path.LastIndexOf('\\') + 1;

                var PathBase = path.Substring(0, idx);

                this.gqplugin = gqplugin;
                this.PathFolder = Directory.GetCurrentDirectory() + "\\wwwroot\\plugins\\" + gqplugin.Folder + "\\";

                AppDomainSetup domaininfo = new AppDomainSetup();
                domaininfo.ApplicationBase = PathFolder;
                domaininfo.PrivateBinPath = PathBase;
                domaininfo.DisallowApplicationBaseProbing = true;
                Evidence adevidence = AppDomain.CurrentDomain.Evidence;
                domain = AppDomain.CreateDomain("Plugins", adevidence, domaininfo);

                var ar = AssemblyResolver.Register(domain, PathFolder, PathBase);
                DirectoryInfo di = new DirectoryInfo(PathFolder);

                foreach (FileInfo file in di.GetFiles("*.dll"))
                {
                    ar.LoadAssembly(file.FullName);
                }
                ar.LoadPlugIn();
                this.Value = ar;
            }

            public void Dispose()
            {
                AppDomain.Unload(domain);
                GC.Collect(); // collects all unused memory
                GC.WaitForPendingFinalizers(); // wait until GC has finished its work
                GC.Collect();
            }
        }

        public static Plugin GetPlugin(Gq_plugin plugin)
        {
            try
            {
                switch (plugin.Tipo)
                {
                    case "I":
                        {
                            var path = Directory.GetCurrentDirectory() + "\\wwwroot\\plugins\\" + plugin.Folder + "\\MEMImportacion.dll";

                            if (File.Exists(path))
                            {
                                return new Plugin(plugin);
                            }
                            break;
                        }
                    case "M":
                        {
                            var path = Directory.GetCurrentDirectory() + "\\wwwroot\\plugins\\" + plugin.Folder + "\\MEMModel.dll";

                            if (!File.Exists(path))
                            {
                                return new Plugin(plugin);
                            }
                            break;
                        }
                }

                return null;

            }
            
            catch(Exception ex) {
                return null;
                
            }

        }
    }
}
