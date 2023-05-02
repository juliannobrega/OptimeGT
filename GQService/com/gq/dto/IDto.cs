using System.Collections.Generic;

namespace GQService.com.gq.dto
{
    public interface IDto<T, E> : IGenericDto where T : IEntity

    {
        IEnumerable<E> SetEntity(IEnumerable<T> values);
        E SetEntity(T value);
        IEnumerable<T> GetEntity(IEnumerable<E> values);
        new T GetEntity();

        /// <summary>
        /// Crea los objetos DTO segun el Entity
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        E makeDto(T values);

        /// <summary>
        /// Crea una lista de objetos DTO segun el Entity
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        List<E> makeDto(List<T> values);
    }
}
