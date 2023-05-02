namespace MEMDataService.com.gq.InterfaceLibreriaExterna
{
    public interface IPlugin
    {
        string[] CommandsList();
        string Command(string comando, params object[] parametros);
        string Version();
    }
}
