using BetterConfig.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterConfig.Storage
{
    public class ConfigurationManagerConfigStore : ConfigStoreBase
    {
        public override IEnumerable<ConfigSetting> ReadAll()
        {
            var result = new List<ConfigSetting>();

            foreach (var key in ConfigurationManager.AppSettings.AllKeys)
            {
                result.Add(new ConfigSetting(key, ConfigurationManager.AppSettings[key]));
            }

            return result;
        }
    }
}
