using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yammer.Chat.Core.API.Dtos;
using Yammer.Chat.Core.Models;

namespace Yammer.Chat.Core.Parsers
{
    public interface IAttachmentParser : IParser<AttachmentDto, Attachment>
    { }

    public class AttachmentParser : IAttachmentParser
    {
        public Attachment[] ToModel(IEnumerable<AttachmentDto> attachmentDtos)
        {
            if (attachmentDtos == null)
                return new Attachment[0];

            return attachmentDtos
                .Where(x => x is ImageAttachmentDto)
                .Select(x => new ImageAttachment
                {
                    Preview = ((ImageAttachmentDto)x).Preview,
                    LargePreview = ((ImageAttachmentDto)x).LargePreview
                })
                .ToArray<Attachment>();
        }

        public Attachment ToModel(AttachmentDto attachmentDto)
        {
            return new Attachment
            {
                Id = attachmentDto.Id
            };
        }

        public AttachmentDto[] ToDto(IEnumerable<Attachment> attachments)
        {
            if (attachments == null)
                return new AttachmentDto[0];

            return attachments
                .Select(x => new AttachmentDto
                {
                    Id = x.Id
                })
                .ToArray();
        }

        public AttachmentDto ToDto(Attachment model)
        {
            throw new NotImplementedException();
        }
    }
}
