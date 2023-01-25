using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yammer.Chat.Core
{
    public interface IProgressIndicator
    {
        IDisposable Show();
        IDisposable Show(string text);
        void Hide();
        bool IsShowing();
    }
}
