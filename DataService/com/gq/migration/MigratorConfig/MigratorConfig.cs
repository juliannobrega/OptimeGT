using System.Reflection;
namespace MEMDataService.com.gq.migrations
{
    public static class MigratorConfig
    {
        public static Assembly GetAssembly()
        {
            return Assembly.GetExecutingAssembly();
        }
    }
}
