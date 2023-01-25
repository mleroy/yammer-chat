using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yammer.Chat.Core
{
    public interface IPhotoChooser
    {
        Task<PhotoChooserResult> GetPhoto();
    }

    public class PhotoChooserResult
    {
        public string Filename { get; set; }
        public Stream Photo { get; set; }
    }
}
