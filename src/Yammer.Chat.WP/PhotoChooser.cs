using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yammer.Chat.Core;

namespace Yammer.Chat.WP.Core
{
    public class PhotoChooser : IPhotoChooser
    {
        public Task<PhotoChooserResult> GetPhoto()
        {
            var tcs = new TaskCompletionSource<PhotoChooserResult>();

            var photoChooserTask = new PhotoChooserTask
            {
                ShowCamera = true
            };

            photoChooserTask.Completed += (s, e) =>
            {
                switch (e.TaskResult)
                {
                    case TaskResult.OK:

                        var result = new PhotoChooserResult
                        {
                            Filename = this.CleanFilename(e.OriginalFileName),
                            Photo = e.ChosenPhoto
                        };

                        tcs.SetResult(result);

                        break;

                    case TaskResult.Cancel:

                        tcs.SetCanceled();

                        break;

                    case TaskResult.None:

                        if (e.Error != null)
                        {
                            tcs.SetException(e.Error);
                        }
                        else
                        {
                            tcs.SetException(new Exception("PhotoChooserTask returned no result and didn't have an error"));
                        }

                        break;
                }
            };

            photoChooserTask.Show();

            return tcs.Task;
        }

        private string CleanFilename(string filename)
        {
            return filename.Substring(filename.LastIndexOf(@"\") + 1);
        }
    }
}
