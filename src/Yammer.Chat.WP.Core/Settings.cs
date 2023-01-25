using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yammer.Chat.Core;

namespace Yammer.Chat.WP.Core
{
    public class Settings : ISettings
    {
        void ISettings.AddOrUpdate(string key, object value)
        {
            this.settings[key] = value;
            this.settings.Save();
        }

        bool ISettings.TryGetValue<T>(string key, out T value)
        {
            return this.settings.TryGetValue<T>(key, out value);
        }

        void ISettings.Remove(params string[] keys)
        {
            foreach (var key in keys)
            {
                var result = this.settings.Remove(key);
            }

            this.settings.Save();
        }

        bool ISettings.ContainsKey(string key)
        {
            return this.settings.Contains(key);
        }

        private IsolatedStorageSettings settings
        {
            get { return IsolatedStorageSettings.ApplicationSettings; }
        }
    }
}
