using GQService.com.gq.encriptation;
using GQService.com.gq.menu;
using GQService.com.gq.security;
using GQService.com.gq.service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.Linq;
using GQService.com.gq.utils;
using GQService.com.gq.exception;
using MEMDataService.com.gq.dto;
using MEMDataService.com.gq.service;
using MEMDataService.com.gq.domain;
using MEMDataService.com.gq.constantes;
using GQService.com.gq.log;

namespace MEM.com.gq.security
{
    public static class Security
    {
        #region ROLES DEFINICION

        /// <summary>
        /// DEFINICION DE ROL ADMINISTRADOR
        /// </summary>
        public const string ROL_ADMI = "Administrador";
        public const string ROL_USUARIO = "Usuario";

        public static readonly string[] ROLES = new string[] { ROL_ADMI , ROL_USUARIO};

        public static bool IsAdmin() {
            return HasRol(ROL_ADMI);
        }

        public static bool IsAdminPerfil(long IdPerfil)
        {
            try
            {
                var perfil = Services.Get<ServGq_perfiles>(Services.statelessSession).findById(IdPerfil);
                var result = perfil.Nombre.ToLower().Contains("admin");
                return result;
            }
            catch (Exception ex)
            {
                Log.Error("Perfil - IsAdminPerfil", ex);
                return false;
            }

        }

        #endregion

        #region MENUES PADRES DEFINICION

        public const string MENU_ADMINISTACION_ESCENARIO = "10-00-00";
        public const string MENU_ADMINISTACION_USUARIO = "20-00-00";
        public const string MENU_CONFIGURACION_AVANZADA = "30-00-00";
        public const string MENU_DESCARGAS = "40-00-00";

        #endregion

        #region SEGURIDAD

        public static Gq_usuariosDto usuarioLogueado
        {
            get
            {
                return GQService.com.gq.security.Security.usuarioLogueado as Gq_usuariosDto;
            }
            set
            {
                GQService.com.gq.security.Security.usuarioLogueado = value;
            }
        }

        public static Gq_perfilesDto getPerfilUserLogueado()
        {
            if (usuarioLogueado != null)
            {
                Gq_perfiles perfil = Services.Get<ServGq_perfiles>().findById(Security.usuarioLogueado.PerfilId);
                return new Gq_perfilesDto().SetEntity(perfil);
            }
            else return null;
        }

        public static string getNameObject(object value)
        {
            return GQService.com.gq.security.Security.getNameObject(value);
        }

        public static bool hasPermission(object value, string method, bool returnException = true)
        {
            bool result = true;
            bool isLogued = Security.usuarioLogueado != null;

            if (isLogued && !CheckUsuarioLoginKey())
            {
                result = false;
            }
            else
            {
                Type type = (Type)(value is Type ? value : value.GetType().GetTypeInfo());

                SecurityDescription attribute = (SecurityDescription)type.GetCustomAttribute(typeof(SecurityDescription), true);
                if (attribute != null)
                {
                    if (attribute.Estado == SecurityDescription.SeguridadEstado.Activo)
                    {
                        result = result && _hasPermission(type, null, returnException);
                    }
                    else if (attribute.Estado == SecurityDescription.SeguridadEstado.SoloLogueo && isLogued == false)
                    {
                        result = false;
                    }

                    if (result && method != null)
                    {
                        var methodFind = ClassUtils.GetMethod(type, method);
                        if (methodFind.Length > 0)
                        {
                            attribute = (SecurityDescription)methodFind[0].GetCustomAttribute(typeof(SecurityDescription), true);
                            if (attribute != null)
                            {
                                if (attribute.Estado == SecurityDescription.SeguridadEstado.Activo)
                                {
                                    result = result && _hasPermission(type, method, returnException);
                                }
                                else if (attribute.Estado == SecurityDescription.SeguridadEstado.SoloLogueo && isLogued == false)
                                {
                                    result = false;
                                }
                            }
                            else
                            {
                                result = false;
                            }
                        }
                        else
                        {
                            result = false;
                        }
                    }
                }
                else
                {
                    result = false;
                }
            }

            if (result == false && returnException)
            {
                throw new SecurityException(value, method);
            }

            return result;
        }

        public static void AddUsuarioLogin(Gq_usuarios user)
        {
            GQService.com.gq.security.Security.AddUsuarioLogin(user.UsuarioId.Value, user.LoginKey);
        }

        public static bool CheckUsuarioLoginKey()
        {
            return GQService.com.gq.security.Security.CheckUsuarioLoginKey(usuarioLogueado.UsuarioId.Value, usuarioLogueado.LoginKey);
        }

        private static List<Gq_accesos> Accesos = null;

        private static List<Gq_accesos> GetAccesos()
        {
            if (Accesos == null)
            {
                Accesos = Services.Get<ServGq_accesos>().findBy().ToList();
            }
            return Accesos;
        }

        private static bool _hasPermission(object value, string method, bool returnException = true)
        {
            bool result = false;
            long perfilId = usuarioLogueado == null ? -1 : usuarioLogueado.PerfilId;

            Gq_accesos accesos = GetAccesos().Where(x => x.Clase == getNameObject(value) && x.Metodo == method).FirstOrDefault();

            var perfil = Services.Get<ServGq_perfiles>().findBy(x => x.PerfilId == perfilId && x.Estado==Constantes.ESTADO_ACTIVO).FirstOrDefault();

            if (perfil != null)
            {
                if (CheckUsuarioLoginKey())
                {
                    var acc = Services.Get<ServGq_perfiles_accesos>().findBy(x => x.PerfilId == perfilId && x.AccesoId == accesos.AccesoId).FirstOrDefault();
                    return (acc == null ? false : (acc.GrantPermition == "1"));
                }
            }

            if (result == false && returnException)
            {
                throw new SecurityException(value, method);
            }
            return result;
        }

        public static bool hasControllerPermission(string value)
        {
            if (string.IsNullOrWhiteSpace(value) == false)
            {
                string accion = string.Empty;
                string controller = string.Empty;

                var array = value.Split('/');

                if (array.Length > 0)
                    controller = "MEM.Controllers." + array[0] + "Controller"; ///TODO Tengo que ver esta Parte del Cotroller

                if (array.Length > 1)
                    accion = array[1];

                bool result = false;

                var q = Services.Get<ServGq_accesos>().findBy(x => x.Clase == controller && ((accion != null && x.Metodo == accion) || (x.Metodo == null)));
                var accesos = q.OrderByDescending(x => x.Metodo).FirstOrDefault();
                var perfil = Services.Get<ServGq_perfiles>().findBy(x => x.PerfilId == usuarioLogueado.PerfilId && x.Estado == Constantes.ESTADO_ACTIVO).FirstOrDefault();

                if (perfil != null)
                {
                    if (accesos != null)
                    {
                        var paList = Services.Get<ServGq_perfiles_accesos>().findBy(x => x.PerfilId == usuarioLogueado.PerfilId && x.AccesoId == accesos.AccesoId.Value).ToList<Gq_perfiles_accesos>();
                        if (paList.Count > 0)
                        {
                            result = true;
                            foreach (var pa in paList)
                            {
                                result = result && pa.GrantPermition == "1";
                            }
                        }
                    }
                }
                
                return result;
            }
            else
            {
                return true;
            }
        }
        
        public static bool HasRol(string rol)
        {
            bool result = false;
            try
            {
                if (usuarioLogueado != null)
                {
                    result = Services.Get<ServGq_perfiles>().findById(usuarioLogueado.PerfilId).Nombre == rol;
                }
            }
            catch
            {

            }
            return result;
        }

        #endregion

        public static void CreateAccessSecurity()
        {
            using (var transaction = Services.session.BeginTransaction())
            {
                Assembly assembly = typeof(Security).GetTypeInfo().Assembly;

                SecurityDescription attribute = null;
                MenuDescription menuAttribute = null;

                Dictionary<string, Gq_perfiles> roles = new Dictionary<string, Gq_perfiles>();
                Dictionary<MenuDescription, object> menues = new Dictionary<MenuDescription, object>();
                ArrayList items = new ArrayList();

                Gq_accesosDto itemClass = null;
                Gq_accesosDto itemMethod = null;

                #region CREACION DE ROLES

                if (ROLES.Length > 0)
                {
                    for (int i = 0; i < ROLES.Length; i++)
                    {
                        if (roles.ContainsKey(ROLES[i]) == false)
                            roles.Add(ROLES[i], null);
                    }
                }

                for (int i = 0; i < roles.Keys.Count; i++)
                {
                    string key = roles.Keys.ElementAt(i);
                    var rol = Services.Get<ServGq_perfiles>().findBy(x => x.KeyName == key).FirstOrDefault();

                    if (rol == null)
                    {
                        rol = new Gq_perfiles();
                        rol.Nombre = key;
                        rol.KeyName = key;
                        rol.Estado = Constantes.ESTADO_ACTIVO;
                        rol.Creado = DateTime.Now;
                        rol.Modificado = DateTime.Now;
                        rol.CreadoPor = -1;
                        rol.ModificadoPor = -1;

                        Services.Get<ServGq_perfiles>().Agregar(rol);

                        rol = Services.Get<ServGq_perfiles>().findBy(x => x.KeyName == key).FirstOrDefault();
                    }

                    roles[key] = rol;
                }
                #endregion

                #region CREACION DE ACCESOS

                var elementsType = (from iface in assembly.GetTypes()
                                    where iface.Namespace != null && iface.Namespace.Contains("Controllers")
                                    select iface);

                foreach (Type item in elementsType)
                {
                    menuAttribute = (MenuDescription)item.GetTypeInfo().GetCustomAttribute(typeof(MenuDescription), false);
                    if (menuAttribute != null)
                    {
                        menues.Add(menuAttribute, item);
                    }
                    attribute = (SecurityDescription)item.GetTypeInfo().GetCustomAttribute(typeof(SecurityDescription), false);

                    if (attribute != null)
                    {
                        itemClass = new Gq_accesosDto();
                        itemClass.Clase = getNameObject(item);

                        if (attribute.Estado == SecurityDescription.SeguridadEstado.Activo)
                        {
                            itemClass.Metodo = null;
                            itemClass.Nombre = attribute.Name;
                            itemClass.extra = attribute;
                            itemClass.Descripcion = "Class :" + attribute.Name;

                            //obtenerProfiles(attribute, roles);

                            items.Add(itemClass);
                        }

                        foreach (MethodInfo method in item.GetMethods())
                        {
                            menuAttribute = (MenuDescription)method.GetCustomAttribute(typeof(MenuDescription), true);
                            if (menuAttribute != null)
                            {
                                menues.Add(menuAttribute, method);
                            }

                            attribute = (SecurityDescription)method.GetCustomAttribute(typeof(SecurityDescription), true);
                            if (attribute != null && attribute.Estado == SecurityDescription.SeguridadEstado.Activo)
                            {
                                itemMethod = new Gq_accesosDto();
                                itemMethod.Clase = itemClass.Clase;
                                itemMethod.Metodo = method.Name;
                                itemMethod.Nombre = attribute.Name;
                                itemMethod.Descripcion = "Method :" + attribute.Name;
                                itemMethod.extra = attribute;

                                //obtenerProfiles(attribute, roles);

                                items.Add(itemMethod);
                            }
                        }
                    }
                }

                foreach (Gq_accesosDto ii in items)
                {
                    var data = Services.Get<ServGq_accesos>().findBy(x => x.Clase == ii.Clase && x.Metodo == ii.Metodo).FirstOrDefault();
                    if (data == null)
                    {
                        data = new Gq_accesos();
                        data.Clase = ii.Clase;
                        data.Metodo = ii.Metodo;
                        data.Descripcion = ii.Descripcion;
                        data.Nombre = ii.Nombre;

                        Services.Get<ServGq_accesos>().Agregar(data);


                        data = Services.Get<ServGq_accesos>().findBy(x => x.Clase == ii.Clase && x.Metodo == ii.Metodo).FirstOrDefault();
                    }
                    else
                    {
                        data.Nombre = ii.Nombre;

                        Services.Get<ServGq_accesos>().Actualizar(data);
                    }

                    attribute = (SecurityDescription)ii.extra;
                    if (attribute.Perfiles != null && attribute.Perfiles.Length > 0)
                    {
                        for (int i = 0; i < attribute.Perfiles.Length; i++)
                        {
                            var dataPerfiles = Services.Get<ServGq_perfiles_accesos>().findBy(x => x.AccesoId == data.AccesoId && x.PerfilId == roles[attribute.Perfiles[i]].PerfilId).FirstOrDefault();
                            if (dataPerfiles == null)
                            {

                                dataPerfiles = new Gq_perfiles_accesos();
                                dataPerfiles.AccesoId = data.AccesoId.Value;
                                dataPerfiles.PerfilId = roles[attribute.Perfiles[i]].PerfilId.Value;
                                dataPerfiles.GrantPermition = "1";
                                dataPerfiles.Estado = "A";
                                dataPerfiles.Creado = DateTime.Now;
                                dataPerfiles.Modificado = DateTime.Now;
                                dataPerfiles.CreadoPor = dataPerfiles.ModificadoPor = -1;

                                Services.Get<ServGq_perfiles_accesos>().Agregar(dataPerfiles);
                            }
                        }
                    }
                }


                #endregion

                #region CREACION DE USUARIO ADMINISTRADOR

                var usuario = Services.Get<ServGq_usuarios>().findBy(x => x.Usuario == "admin").FirstOrDefault();

                if (usuario == null)
                {
                    usuario = new Gq_usuarios
                    {
                        Nombre = "Admin",
                        Apellido = "Admin",
                        Usuario = "admin",
                        Clave = Encriptacion.Encriptar("admin1234", Constantes.CLAVE_ENCRIPTACION),
                        Email = "",
                        Estado = Constantes.ESTADO_ACTIVO,
                        PerfilId = roles[ROL_ADMI].PerfilId.Value,
                        Creado = DateTime.Now,
                        Modificado = DateTime.Now,
                        CreadoPor = -1,
                        ModificadoPor = -1,
                        RequiereClave = "N",
                        GrupoEmpresario = -1
                    };

                    Services.Get<ServGq_usuarios>().Agregar(usuario);

                }


                #endregion

                #region CREACION DE MENUES

                var menu = Services.Get<ServGq_menu>().findBy(x => x.KeyName == MENU_ADMINISTACION_ESCENARIO).FirstOrDefault();

                if (menu == null)
                {
                    menu = new Gq_menu();
                    menu.MenuPosition = MENU_ADMINISTACION_ESCENARIO;
                    menu.KeyName = MENU_ADMINISTACION_ESCENARIO;
                    menu.Nombre = "Administración de Escenarios";
                    menu.MenuIcono = "fa fa-database";
                    menu.Creado = DateTime.Now;
                    menu.Modificado = DateTime.Now;
                    menu.CreadoPor = -1;
                    menu.ModificadoPor = -1;
                    menu.Estado = Constantes.ESTADO_ACTIVO;

                    Services.Get<ServGq_menu>().Agregar(menu);
                }

                menu = Services.Get<ServGq_menu>().findBy(x => x.KeyName == MENU_ADMINISTACION_USUARIO).FirstOrDefault();

                if (menu == null)
                {
                    menu = new Gq_menu();
                    menu.MenuPosition = MENU_ADMINISTACION_USUARIO;
                    menu.KeyName = MENU_ADMINISTACION_USUARIO;
                    menu.Nombre = "Administración de Usuarios";
                    menu.MenuIcono = "fa fa-address-card-o";
                    menu.Creado = DateTime.Now;
                    menu.Modificado = DateTime.Now;
                    menu.CreadoPor = -1;
                    menu.ModificadoPor = -1;
                    menu.Estado = Constantes.ESTADO_ACTIVO;

                    Services.Get<ServGq_menu>().Agregar(menu);
                }

                menu = Services.Get<ServGq_menu>().findBy(x => x.KeyName == MENU_CONFIGURACION_AVANZADA).FirstOrDefault();

                if (menu == null)
                {
                    menu = new Gq_menu();
                    menu.MenuPosition = MENU_CONFIGURACION_AVANZADA;
                    menu.KeyName = MENU_CONFIGURACION_AVANZADA;
                    menu.Nombre = "Configuración Avanzada";
                    menu.MenuIcono = "fa fa-cog";
                    menu.Creado = DateTime.Now;
                    menu.Modificado = DateTime.Now;
                    menu.CreadoPor = -1;
                    menu.ModificadoPor = -1;
                    menu.Estado = Constantes.ESTADO_ACTIVO;

                    Services.Get<ServGq_menu>().Agregar(menu);
                }

                menu = Services.Get<ServGq_menu>().findBy(x => x.KeyName == MENU_DESCARGAS).FirstOrDefault();

                if (menu == null)
                {
                    menu = new Gq_menu();
                    menu.MenuPosition = MENU_DESCARGAS;
                    menu.KeyName = MENU_DESCARGAS;
                    menu.Nombre = "Descargas";
                    menu.MenuIcono = "fa fa-cog";
                    menu.Creado = DateTime.Now;
                    menu.Modificado = DateTime.Now;
                    menu.CreadoPor = -1;
                    menu.ModificadoPor = -1;
                    menu.Estado = Constantes.ESTADO_ACTIVO;

                    Services.Get<ServGq_menu>().Agregar(menu);
                }

                foreach (var m in menues.Keys)
                {
                    menu = Services.Get<ServGq_menu>().findBy(x => x.KeyName == m.Id).FirstOrDefault();
                    if (menu == null)
                    {
                        var obj = menues[m];

                        menu = new Gq_menu();
                        menu.MenuPosition = m.Id;
                        menu.Nombre = m.Description;
                        menu.KeyName = m.Id;
                        menu.MenuIcono = "";
                        menu.MenuPadre = m.IdParent;
                        menu.Creado = DateTime.Now;
                        menu.Modificado = DateTime.Now;
                        menu.CreadoPor = -1;
                        menu.ModificadoPor = -1;
                        menu.Estado = Constantes.ESTADO_ACTIVO;

                        if (obj is Type)
                            menu.MenuUrl = ((Type)obj).Name.Replace("Controller", "");
                        else if (obj is MethodInfo)
                            menu.MenuUrl = ((MethodInfo)obj).DeclaringType.Name.Replace("Controller", "") + @"/" + ((MethodInfo)obj).Name;

                        Services.Get<ServGq_menu>().Agregar(menu);
                    }
                }
                #endregion

                transaction.Commit();
            }
        }
    }
}