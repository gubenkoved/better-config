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

        public ConfigSettingScope Scope { get; set; }

        public ConfigClient(ConfigStoreBase configStore)
        {
            ConfigStore = configStore;
        }

        public string Get(string key)
        {
            var setting = 
                ConfigStore.ReadAll()
                .Single(x => x.Key == key);

            return setting.Definition;
        }
    }
}
