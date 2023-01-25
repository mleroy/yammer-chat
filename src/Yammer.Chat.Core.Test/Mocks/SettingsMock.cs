using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yammer.Chat.Core.Test.Mocks
{
    public class SettingsMock : ISettings
    {
        private Dictionary<string, object> backingStore;

        public SettingsMock()
        {
            this.backingStore = new Dictionary<string, object>();
        }

        public void AddOrUpdate(string key, object value)
        {
            if (this.backingStore.ContainsKey(key))
            {
                this.backingStore[key] = value;
            }
            else
            {
                this.backingStore.Add(key, value);
            }
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            var result = false;

            if (this.backingStore.ContainsKey(key))
            {
                try
                {
                    value = (T)this.backingStore[key];
                    result = true;
                }
                catch (InvalidCastException)
                {
                    value = default(T);
                }
            }
            else
            {
                value = default(T);
            }

            return result;
        }

        public void Remove(params string[] keys)
        {
            foreach (var key in keys)
            {
                this.backingStore.Remove(key);
            }
        }

        public bool ContainsKey(string key)
        {
            return this.backingStore.ContainsKey(key);
        }
    }
}
