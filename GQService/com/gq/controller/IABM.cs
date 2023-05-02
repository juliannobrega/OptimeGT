using GQService.com.gq.controller;
using GQService.com.gq.data;
using GQService.com.gq.dto;

namespace GQService.com.gq.controller
{
    public interface IABM<T>: IBuscar, ICrearModificar<T>, IBorrar<T>
    {
    
    }
}
