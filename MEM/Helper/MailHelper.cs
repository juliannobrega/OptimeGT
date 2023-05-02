using GQService.com.gq.log;
using GQService.com.gq.service;
using MEM.com.gq.mailTemplate;
using MEMDataService.com.gq.domain;
using MEMDataService.com.gq.service;
using System;
using static MEM.com.gq.mailTemplate.ProcesarMailTemplate;

namespace MEM.Helper
{
    public static class MailHelper
    {
        #region Configuración SMTP
        private static Gq_smtp_config getConfig()
        {
            return Services.Get<ServGq_smtp_config>().findByOne(x => x.Nombre.Contains("gmail"));
        }
        #endregion
                
        public static bool Enviar_RecuperarClave(Gq_usuarios pUsuario, string pClave)
        {
            try
            {
                EjecutarDto ejecutar = new EjecutarDto();
                ejecutar.Metodo = "Enviar_Mail";
                ejecutar.Parametros = new object[] { pUsuario, pClave };
                ejecutar.Id = "Clave_recuperada";
                return (bool)ProcesarMailTemplate.Ejecutar(ejecutar);
            }
            catch (Exception ex)
            {
                Log.Error("Enviar_RecuperarClave", ex);
                return false;
            }
        }

    }
}
