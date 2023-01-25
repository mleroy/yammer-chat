using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yammer.Chat.Core.API.Dtos;
using Yammer.Chat.Core.Models;

namespace Yammer.Chat.Core.Parsers
{
    public interface IUserParser : IParser<UserDto, User>
    {
        User ToModel(UserReferenceDto referenceDto);
        User ToModel(ParticipantDto referenceDto);
        User[] ToModel(IEnumerable<AutoCompleteUserDto> dto);
        User ToModel(AutoCompleteUserDto dto);
    }

    public class UserParser : IUserParser
    {
        public UserDto[] ToDto(IEnumerable<User> models)
        {
            throw new NotImplementedException();
        }

        public UserDto ToDto(User model)
        {
            throw new NotImplementedException();
        }

        public User[] ToModel(IEnumerable<UserDto> dtos)
        {
            return dtos.Select(x => this.ToModel(x)).ToArray();
        }

        public User ToModel(UserDto dto)
        {
            var model = new User
            {
                Id = dto.Id,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                FullName = dto.FullName,
                MugshotTemplate = dto.MugshotTemplate,
                Summary = dto.Summary,
                JobTitle = dto.JobTitle
            };

            if (dto.ContactInfo != null && dto.ContactInfo.PhoneNumbers != null)
            {
                var mobilePhone = dto.ContactInfo.PhoneNumbers.FirstOrDefault(x => x.Type == "mobile");

                if (mobilePhone != null)
                {
                    model.MobilePhone = mobilePhone.Number;
                }

                var workPhone = dto.ContactInfo.PhoneNumbers.FirstOrDefault(x => x.Type == "work");

                if (workPhone != null)
                {
                    model.WorkPhone = workPhone.Number;
                }
            }

            if (dto.Presence != null)
            {
                UserPresence presence;

                if (Enum.TryParse(dto.Presence.Status, true, out presence))
                {
                    model.Presence = presence;
                }
            }

            return model;
        }

        public User ToModel(UserReferenceDto referenceDto)
        {
            var model = new User
            {
                Id = referenceDto.Id,
                FullName = referenceDto.FullName,
                MugshotTemplate = referenceDto.MugshotTemplate
            };

            return model;
        }

        public User ToModel(ParticipantDto referenceDto)
        {
            var model = new User
            {
                Id = referenceDto.Id,
                FirstName = referenceDto.FirstName,
                LastName = referenceDto.LastName,
                FullName = referenceDto.FullName,
                MugshotTemplate = string.IsNullOrEmpty(referenceDto.MugshotUrl) ? string.Empty : referenceDto.MugshotUrl.Replace("48x48", "{width}x{height}")
            };

            return model;
        }

        public User[] ToModel(IEnumerable<AutoCompleteUserDto> dtos)
        {
            return dtos.Select(x => this.ToModel(x)).ToArray();
        }

        public User ToModel(AutoCompleteUserDto dto)
        {
            var model = new User
            {
                Id = dto.Id,
                FullName = dto.FullName,
                MugshotTemplate = dto.MugshotTemplate,
                JobTitle = dto.JobTitle,
            };

            UserPresence presence;
            if (Enum.TryParse(dto.Presence, true, out presence))
            {
                model.Presence = presence;
            }

            return model;
        }
    }
}
