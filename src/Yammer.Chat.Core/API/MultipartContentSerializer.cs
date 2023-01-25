using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Yammer.Chat.Core.API
{
    public class MultipartContentSerializer : IApiSerializer
    {
        public HttpContent Serialize(object obj)
        {
            var package = obj as MultipartPackage;

            if (package == null)
                return null;

            var multipartContent = new MultipartFormDataContent();

            if (package.KeyValuePairs != null && package.KeyValuePairs.Any())
            {
                foreach (var kv in package.KeyValuePairs)
                {
                    multipartContent.Add(new StringContent(kv.Value), kv.Key);
                }
            }

            if (package.Files != null && package.Files.Any())
            {
                foreach (var file in package.Files)
                {
                    var part = new StreamContent(file.Stream);

                    if (!string.IsNullOrEmpty(file.ContentType))
                    {
                        part.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                    }

                    multipartContent.Add(part, file.Name, file.Filename);
                }
            }


            return multipartContent;
        }
    }

    public class MultipartPackage
    {
        public IEnumerable<MultipartFile> Files { get; set; }
        public IEnumerable<KeyValuePair<string, string>> KeyValuePairs { get; set; }
    }

    public class MultipartFile
    {
        public string Name { get; set; }
        public string ContentType { get; set; }
        public Stream Stream { get; set; }
        public string Filename { get; set; }
    }
}
