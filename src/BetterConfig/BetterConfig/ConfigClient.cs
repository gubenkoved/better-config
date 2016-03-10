using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterConfig
{
    public class ConfigClient
    {
        public ConfigStoreBase ConfigStore { get; private set; }

        public string Environment { get; set; }

        public ConfigClient(ConfigStoreBase configStore)
        {
            ConfigStore = configStore;
        }

        public string Get(string key)
        {
            var setting = 
                ConfigStore.ReadAll()
                .Single(x => x.Environemnt == Environment
                    && x.Key == key);

            return CurrentValueFor(setting);
        }

        private string CurrentValueFor(ConfigSetting setting)
        {
            return setting
                .Values
                .OrderByDescending(x => x.EffectiveSinceUtc ?? DateTime.MinValue)
                .First(x => (x.EffectiveSinceUtc ?? DateTime.MinValue) <= DateTime.UtcNow)
                .Value;
        }
    }
}
