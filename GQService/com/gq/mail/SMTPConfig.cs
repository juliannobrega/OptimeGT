
namespace GQService.com.gq.mail
{
    public interface ISMTPConfig
    {
        System.String Nombre { get; set; }


        System.String NombreFrom { get; set; }


        System.String Pass { get; set; }


        System.String Host { get; set; }


        System.Int32 Port { get; set; }


        System.Boolean UseDefaultCredentials { get; set; }


        System.Boolean EnableSsl { get; set; }


        System.String EMailFrom { get; set; }
    }
}
