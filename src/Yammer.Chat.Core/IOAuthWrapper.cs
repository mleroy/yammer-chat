using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yammer.Chat.Core
{
    public interface IOAuthWrapper
    {
        void LaunchLogin();

        void HandleApprove(string code,
           string state,
           Action<string, long> onSuccess,
           Action onCSRF = null,
           Action<string> onErrorResponse = null,
           Action<Exception> onException = null);

        void DeleteStoredToken();
    }
}
