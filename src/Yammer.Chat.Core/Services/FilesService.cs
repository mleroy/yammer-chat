using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Yammer.Chat.Core.API;
using Yammer.Chat.Core.API.Dtos;
using Yammer.Chat.Core.Repositories;

namespace Yammer.Chat.Core.Services
{
    public interface IFilesService
    {
        Task<AttachmentDto> UploadImage(PhotoChooserResult photo);
        Task<MugshotDto> UploadMugshot(PhotoChooserResult photo);
    }

    public class FilesService : IFilesService
    {
        private readonly IIdentityStore identityStore;
        private readonly IApiService apiService;

        public FilesService(IIdentityStore identityStore, IApiService apiService)
        {
            this.identityStore = identityStore;
            this.apiService = apiService;
        }

        public async Task<AttachmentDto> UploadImage(PhotoChooserResult photo)
        {
            var parameters = new MultipartPackage();

            parameters.Files = new MultipartFile[]
            {
                new MultipartFile 
                {
                    Name = "file",
                    Stream = photo.Photo,
                    Filename = photo.Filename
                }
            };

            var settings = new ApiRequestSettings(new MultipartContentSerializer());

            var response = await this.apiService.PostAsync("https://files.yammer.com/v2/files", parameters, settings);

            return response.ToEntity<AttachmentDto>();
        }

        public async Task<MugshotDto> UploadMugshot(PhotoChooserResult photo)
        {
            var parameters = new MultipartPackage();

            parameters.Files = new MultipartFile[]
            {
                new MultipartFile 
                {
                    Name = "image", 
                    ContentType = this.GetMugshotContentType(photo.Filename),
                    Stream = photo.Photo,
                    Filename = photo.Filename
                }
            };

            parameters.KeyValuePairs = new KeyValuePair<string, string>[] 
            {
                new KeyValuePair<string, string>("oauth_token", this.identityStore.Token),
                new KeyValuePair<string, string>("csrf_token", "DpPk9hIOkhMT8ZY4GL7Eg")
            };

            var settings = new ApiRequestSettings(new MultipartContentSerializer());

            var response = await this.apiService.PostAsync("/mugshot/images/", parameters, settings);

            return response.ToEntity<MugshotDto>();
        }

        private string GetMugshotContentType(string filename)
        {
            if (!string.IsNullOrEmpty(filename))
            {
                if (filename.EndsWith("jpg") || filename.EndsWith("jpeg") || filename.EndsWith("jpe"))
                {
                    return "image/jpeg";
                }
                else if (filename.EndsWith("png"))
                {
                    return "image/png";
                }
                else if (filename.EndsWith("tiff") || filename.EndsWith("tif"))
                {
                    return "image/tif";
                }
                else if (filename.EndsWith("gif"))
                {
                    return "image/gif";
                }
            }

            // Don't know
            return "image/png";
        }
    }
}
