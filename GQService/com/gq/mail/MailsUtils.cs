using System;
using System.Collections.Generic;
using MimeKit;
using MailKit;
using MailKit.Security;
using MailKit.Net.Smtp;
using GQService.com.gq.log;

namespace GQService.com.gq.mail
{
    public static class MailsUtils
    {
        public static bool EnviarMail(List<String> to, string subject, String body, ISMTPConfig config)
        {
            return EnviarMail(to, null, null, subject, body, config);
        }
        public static bool EnviarMail(List<String> to, List<String> cc, List<String> co, string subject, String body, ISMTPConfig config)
        {

            try
            {
                #region Creación de Mensaje Mail

                var msg = new MimeMessage();
                msg.From.Add(new MailboxAddress(config.NombreFrom, config.EMailFrom));

                if (to != null) foreach (var item in to) { msg.To.Add(new MailboxAddress(item, item)); }

                if (cc != null) foreach (var item in cc) { msg.Cc.Add(new MailboxAddress(item, item)); }

                if (co != null) foreach (var item in co) { msg.Bcc.Add(new MailboxAddress(item, item)); }

                msg.Subject = subject;
                msg.Body = new TextPart("html") { Text = body };

                #endregion

                #region Creación de Cliente SMTP y Envío

                using (var client = new SmtpClient(new ProtocolLogger("smtp.log")))
                {
                    try
                    {
                        Log.Info("MailUtils_EnviarMail Entro");
                        client.Connect(config.Host, config.Port, SecureSocketOptions.SslOnConnect);
                        Log.Info("MailUtils_EnviarMail config.Host, config.Port");
                        client.Authenticate(config.EMailFrom, config.Pass);
                        Log.Info("MailUtils_EnviarMail Authenticate");
                        client.Send(msg);
                        Log.Info("MailUtils_EnviarMail Send");
                        client.Disconnect(true);
                        Log.Info("MailUtils_EnviarMail Disconnect");
                    }
                    catch (Exception ex)
                    {
                        Log.Error("MailUtils_EnviarMail", ex);
                        //throw;
                    }
                   
                }

                #endregion
            }
            catch (Exception ex)
            {
                Log.Error("MailUtils_EnviarMail", ex);
                return false;
            }

            return true;

        }
    }
}
