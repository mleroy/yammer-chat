using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yammer.Chat.Core.Models;
using Yammer.Chat.Core.Parsers;
using Yammer.Chat.Core.Services;

namespace Yammer.Chat.Core.Repositories
{
    public interface IFileRepository
    {
        Task<Attachment> UploadImage(PhotoChooserResult photo);
        Task<Mugshot> UploadMugshot(PhotoChooserResult photo);
    }

    public class FileRepository : IFileRepository
    {
        private readonly IFilesService filesService;
        private readonly IAttachmentParser attachmentParser;

        public FileRepository(IFilesService filesService, IAttachmentParser attachmentParser)
        {
            this.filesService = filesService;
            this.attachmentParser = attachmentParser;
        }

        public async Task<Attachment> UploadImage(PhotoChooserResult photo)
        {
            var attachmentDto = await this.filesService.UploadImage(photo);

            return this.attachmentParser.ToModel(attachmentDto);
        }

        public async Task<Mugshot> UploadMugshot(PhotoChooserResult photo)
        {
            var dto = await this.filesService.UploadMugshot(photo);

            return new Mugshot { Id = dto.Id.ToString() };
        }
    }
}
