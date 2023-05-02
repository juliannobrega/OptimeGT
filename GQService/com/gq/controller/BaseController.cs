using GQService.com.gq.data;
using GQService.com.gq.exception;
using GQService.com.gq.log;
using GQService.com.gq.security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GQService.com.gq.controller
{
    public class BaseController : Controller
    {
        public string GetRequestData()
        {
            string result = "";
            try
            {
                Stream req = Request.Body;
                req.Seek(0, System.IO.SeekOrigin.Begin);
                result = new StreamReader(req).ReadToEnd();
            }
            catch (Exception e)
            {
                Log.Error(this, "GetRequestData", e);
            }
            return result;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            MethodInfo method = null;

            method = GetMethod(context.RouteData.Values["action"].ToString());

            //Log.Debug(this, method.Name + " | " + GetRequestData());
            //TODO cuando pasemos a producción, eso se debe solucionar

            IActionResult result = hasPermission(method, context);
            if (result != null)
            {
                context.Result = result;
            }

            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            MethodInfo method = GetMethod(context.RouteData.Values["action"].ToString());

            if (method.ReturnType.Equals(typeof(IActionResult)) == false)
            {
                if (method.ReturnType.Equals(typeof(ReturnData)) == false)
                {
                    if (context.Result == null)
                    {
                        if (context.Exception != null)
                        {
                            context.Result = Json(new ReturnData { isError = true, data = new GenericError(context.Exception) });
                            Log.Error(this, method.Name, context.Exception);
                        }
                        else
                        {
                            context.Result = Json(new ReturnData { data = null });
                        }

                    }
                    else
                    {
                        if (context.Result is ObjectResult)
                        {
                            try
                            {
                                if (((ObjectResult)context.Result).Value is ReturnData)
                                {
                                    context.Result = Json(((ObjectResult)context.Result).Value);
                                }
                                else
                                {
                                    context.Result = Json(new ReturnData { data = ((ObjectResult)context.Result).Value });
                                }
                            }
                            catch (Exception ex)
                            {
                                context.Result = Json(new ReturnData { isError = true, data = new GenericError(ex) });
                                Log.Error(this, method.Name, ex);
                            }
                        }
                    }
                }
            }
            base.OnActionExecuted(context);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isPartial"></param>
        /// <returns></returns>
        protected IActionResult hasPermission(MethodInfo value, ActionExecutingContext context)
        {
            string method = value.Name;

            SecurityDescription attributeClass = (SecurityDescription)this.GetType().GetTypeInfo().GetCustomAttribute(typeof(SecurityDescription), true);
            SecurityDescription attributeMethod = (SecurityDescription)value.GetCustomAttribute(typeof(SecurityDescription), true);

            if (method.Equals("Index"))
                method = null;

            if (Security.usuarioLogueado == null && Security.IsExcludeController(Security.getNameObject(this)) == false)
            {
                Log.Info("Redirected 1 " + Security.getNameObject(this));

                if (value.ReturnType.Equals(typeof(IActionResult)))
                {
                    return RedirectToAction("Redirected", "Login");
                }
                else
                {
                    return Json(new ReturnData { isError = true, isSecurity = true, data = "Redirected" });
                }
            }

            if (!Security.hasPermission(this, method, false))
            {
                if (Security.IsExcludeController(Security.getNameObject(this)) == false)
                {
                    Log.Info("Redirected 2 " + Security.getNameObject(this));
                    if (value.ReturnType.Equals(typeof(IActionResult)))
                    {
                        if (Security.CheckUsuarioLoginKey())
                            return RedirectToAction("Index", "Home");
                        else
                            return RedirectToAction("Redirected", "Login");
                    }
                    else
                    {
                        return Json(new ReturnData { isError = true, isSecurity = true, data = "Redirected" });
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected MethodInfo GetMethod(string name)
        {
            Type type = this.GetType();
            var method = (from iface in type.GetMethods()
                          where iface.Name.Equals(name)
                          select iface).ToArray();

            return method[0];

        }
    }
}
