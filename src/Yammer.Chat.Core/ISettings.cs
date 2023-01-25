using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yammer.Chat.Core
{
    public interface ISettings
    {
        void AddOrUpdate(string key, object value);
        bool TryGetValue<T>(string key, out T value);
        void Remove(params string[] keys);
        bool ContainsKey(string key);
    }
}
