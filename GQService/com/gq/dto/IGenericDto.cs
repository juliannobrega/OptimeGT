using System.Collections.Generic;

namespace GQService.com.gq.dto
{
    public interface IGenericDto
    {
        IEnumerable<IGenericDto> SetEntity(IEnumerable<IEntity> values);
        IGenericDto SetEntity(IEntity value);
        IEnumerable<IEntity> GetEntity(IEnumerable<IGenericDto> values);
        IEntity GetEntity();
    }
}
