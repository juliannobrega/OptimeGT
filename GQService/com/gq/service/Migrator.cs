using FluentMigrator;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Initialization;
using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace GQService.com.gq.service
{
    /// <summary>
    /// 
    /// </summary>
    public class Migrator
    {
        private readonly string _connectionString;
        private readonly string _dbType;
        private readonly Assembly _assembly;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="dbType"></param>
        /// <param name="assembly"></param>
        public Migrator(string connectionString, string dbType, Assembly assembly)
        {
            _connectionString = connectionString;
            _dbType = dbType;
            _assembly = assembly;
        }

        /// <summary>
        /// 
        /// </summary>
        private class MigrationOptions : IMigrationProcessorOptions
        {
            public bool PreviewOnly { get; set; }
            public int Timeout { get; set; }
            public string ProviderSwitches { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="runnerAction"></param>
        public void Migrate(Action<IMigrationRunner> runnerAction)
        {
            var announcer = new TextWriterAnnouncer(s => System.Diagnostics.Debug.WriteLine(s));

            var options = new MigrationOptions { PreviewOnly = false, Timeout = 7200 };

            string dbType = "";

            if (_dbType.Contains("MySql"))
            {
                dbType = "MySql";
            }
            else if (_dbType.Contains("SqlClient"))
            {
                dbType = "SqlServer";
            }
            else if (_dbType.Contains("SQLite"))
            {
                dbType = "SQLite";
            }

            var factory = new FluentMigrator.Runner.Processors.MigrationProcessorFactoryProvider().GetFactory(dbType);

            //var assembly = Assembly.GetExecutingAssembly();


            var migrationContext = new RunnerContext(announcer);
            var processor = factory.Create(_connectionString, announcer, options);
            var runner = new MigrationRunner(_assembly, migrationContext, processor);
            runnerAction(runner);
        }

        /* /// <summary>
         /// 
         /// </summary>
         /// <param name="assemblyName"></param>
         public static void Start(string assemblyName)
         {
             Assembly assembly = utils.ClassUtils.getAssembyByName(assemblyName);
             Start(assembly);
         }*/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        public static void Start(Assembly assembly)
        {
            var providerName = getProviderName();
            var connectionString = getConectionString();
            var migrator = new Migrator(connectionString, providerName, assembly);

            migrator.Migrate(runner => runner.MigrateUp());
        }

        protected static IConfigurationSection configurationSection;
        public static void Configure(IConfigurationSection configurationSection)
        {
            Migrator.configurationSection = configurationSection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string getConectionString()
        {
            return configurationSection.GetSection("ConnectionString").Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string getProviderName()
        {
            return configurationSection.GetSection("ProviderName").Value;
        }
    }
}