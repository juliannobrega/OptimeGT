using GQService.com.gq.controller;
using GQService.com.gq.security;
using GQService.com.gq.service;
using MEMDataService.com.gq.constantes;
using MEMDataService.com.gq.dto;
using MEMDataService.com.gq.service;
using System.Collections.Generic;
using System.Linq;

namespace MEM.Controllers
{
    [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
    public class MenuController : BaseController
    {
        [SecurityDescription(SecurityDescription.SeguridadEstado.SoloLogueo)]
        public List<Gq_menuDto> Buscar()
        {
            List<Gq_menuDto> roots = new List<Gq_menuDto>();
            var query = new Gq_menuDto().SetEntity(Services.Get<ServGq_menu>().findBy(x => x.Estado == Constantes.ESTADO_ACTIVO).OrderBy(x => x.MenuPosition).ToList());
            foreach (var item in query)
            {
                if (string.IsNullOrWhiteSpace(((Gq_menuDto)item).MenuPadre))
                {
                    var items = query.Where(x => x.MenuPadre == ((Gq_menuDto)item).MenuPosition).OrderBy(x => x.MenuPosition).ToList();

                    foreach(var mi in items)
                    {
                        if (MEM.com.gq.security.Security.hasControllerPermission(mi.MenuUrl) == true)
                        {
                            ((Gq_menuDto)item).Child.Add(mi);
                        }
                    }

                    if(((Gq_menuDto)item).Child.Count>0)
                        roots.Add(((Gq_menuDto)item));
                }
            }
            return roots;
        }

    }
}
