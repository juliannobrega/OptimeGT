using GQService.com.gq.data;

namespace GQService.com.gq.controller
{
    
    public interface IBorrar<T>
    {
        ReturnData Borrar(T model);
    }
}
