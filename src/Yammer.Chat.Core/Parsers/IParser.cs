using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yammer.Chat.Core.Parsers
{
    public interface IParser<TDto, TModel>
    {
        TDto[] ToDto(IEnumerable<TModel> models);
        TDto ToDto(TModel model);

        TModel[] ToModel(IEnumerable<TDto> dtos);
        TModel ToModel(TDto dto);
    }
}
