using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterConfig
{
    public static class EffectiveSettingsEvaluator
    {
        public static IEnumerable<ConfigSetting> GetEffective(
            IEnumerable<ConfigSetting> allSettings, ConfigSettingScope targetScope)
        {
            // algorithm:
            // 1. create map for effective settings
            // 2. for each x in settings
            //      add x to map
            //      
            //  if the item with key is already in map there,
            //      then override if scope priority is bigger
            //      otherwise skip

            var effective = new Dictionary<string, ConfigSetting>();

            foreach (var item in allSettings)
            {

            }

            return effective.Values;
        }
    }
}
