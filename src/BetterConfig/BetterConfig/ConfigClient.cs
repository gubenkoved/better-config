using System;
using System.Collections.Generic;
using BetterConfig.Core;
using BetterConfig.Storage;

namespace BetterConfig
{
    public class ConfigClient : ConfigClientBase
    {
        private DateTime _lastUpdateUtc = DateTime.MinValue;
        private Dictionary<string, string> _cachedValues;

        /// <summary>
        /// Gets or sets TTL for config values.
        /// As soon as TTL is passed Config Store will be queried again for values.
        /// </summary>
        public TimeSpan TTL { get; set; } = TimeSpan.FromMinutes(5);

        public ConfigClient(ConfigStoreBase configStore, ConfigSettingScope scope)
            :base(configStore, scope)
        {
        }

        public override Dictionary<string, string> GetAll()
        {
            if (DateTime.UtcNow - _lastUpdateUtc > TTL)
            {
                _cachedValues = base.GetAll();

                _lastUpdateUtc = DateTime.UtcNow;
            }

            return _cachedValues;
        }
    }
}
