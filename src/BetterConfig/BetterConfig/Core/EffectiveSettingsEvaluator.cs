using System.Collections.Generic;
using BetterConfig.Exceptions;

namespace BetterConfig.Core
{
    public static class EffectiveSettingsEvaluator
    {
        public static Dictionary<string, ConfigSetting> GetEffectiveAsDict(
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
                if (item.Scope.IsApplicableTo(targetScope))
                {
                    if (!effective.ContainsKey(item.Key))
                    {
                        effective[item.Key] = item;
                    }
                    else // override if bigger priority
                    {
                        if (item.Scope.Priority == effective[item.Key].Scope.Priority)
                            throw new BetterConfigException($"Ambiguity between following two settings found: '{item}' and '{effective[item.Key]}'");

                        if (item.Scope.Priority > effective[item.Key].Scope.Priority)
                            effective[item.Key] = item;
                    }
                }
            }

            return effective;
        }

        public static IEnumerable<ConfigSetting> GetEffective(
            IEnumerable<ConfigSetting> allSettings, ConfigSettingScope targetScope)
        {
            return GetEffectiveAsDict(allSettings, targetScope).Values;
        }
    }
}
