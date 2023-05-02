using GQService.com.gq.log;
using GQService.com.gq.mail;
using GQService.com.gq.service;
using MEMDataService.com.gq.domain;
using MEMDataService.com.gq.service;
using System;
using System.Collections.Generic;

public class Main
{
    public bool Enviar_Mail(Gq_usuarios pUsuario, string pClave)
    {
        try
        {
            String asunto = "Recuperar Clave";
            var dir = System.IO.Directory.GetCurrentDirectory();
            String body = System.IO.File.ReadAllText(dir + "\\wwwroot\\mailTemplate\\Clave_recuperada\\mailTemplate.html");

            body = body.Replace("{NombreYApellido}", pUsuario.Nombre + " " + pUsuario.Apellido);
            body = body.Replace("{Usuario}", pUsuario.Usuario);
            body = body.Replace("{Clave}", pUsuario.Clave);
            body = body.Replace("{Email}", pUsuario.Email);

            List<string> lstTo = new List<string>(new string[] { pUsuario.Email });
            lstTo.Add(pUsuario.Email);
            return MailsUtils.EnviarMail(lstTo, asunto, body, getConfig());
        }
        catch (Exception ex)
        {
            Log.Error("Enviar_Mail", ex);
            return false;
        }
    }

    #region Configuracion SMTP
    private Gq_smtp_config getConfig()
    {
        return Services.Get<ServGq_smtp_config>().findByOne(x => x.Nombre.Contains("gmail"));
    }
    #endregion
}