using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ProviderInitiatedSingleSignOn.Services
{
    public sealed class StateService
    {
        readonly ConcurrentDictionary<string, IDictionary<string, string>> _cache = new ConcurrentDictionary<string, IDictionary<string, string>>();

        public string Create(IDictionary<string, string> properties)
        {
            var key = Guid.NewGuid().ToString().Replace("{", "").Replace("}", "").Replace("-", "");

            _cache.TryAdd(key, properties);

            return key;
        }

        public bool TryValidate(string key, out IDictionary<string, string> properties)
        {
            return _cache.TryRemove(key, out properties);
        }
    }
}