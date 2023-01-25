using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yammer.Chat.Core.API.Dtos;
using Yammer.Chat.Core.Models;

namespace Yammer.Chat.Core.Parsers
{
    public interface IParticipantParser
    {
        ParticipantDto[] ToDto(IEnumerable<User> models);
    }

    public class ParticipantParser : IParticipantParser
    {
        public ParticipantDto[] ToDto(IEnumerable<User> models)
        {
            if (models == null)
                return new ParticipantDto[0];

            return models
                .Select(x => new ParticipantDto
                {
                    Id = x.Id
                })
                .ToArray();
        }
    }
}
