using GQService.com.gq.data;

namespace GQService.com.gq.controller
{
    public interface ICrearModificar<T>
    {
        ReturnData Guardar(T model);
    }
}
