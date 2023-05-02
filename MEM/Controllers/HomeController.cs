using GQService.com.gq.controller;
using GQService.com.gq.security;
using Microsoft.AspNetCore.Mvc;

namespace MEM.Controllers
{
    [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
    public class HomeController : BaseController
    {
        [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
        public IActionResult Index()
        {
            return View();
        }

        [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
