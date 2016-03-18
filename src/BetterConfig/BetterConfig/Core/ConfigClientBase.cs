using BetterConfig.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterConfig.Core
{
    public abstract class ConfigClientBase
    {
        private ISettingsInterpolator _interpolator = new SettingsInterpolator();

        public ConfigStoreBase ConfigStore { get; private set; }

        public ConfigSettingScope Scope { get; private set; }

        public ISettingsInterpolator Interpolator
        {
            get
            {
                return _interpolator;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                _interpolator = value;
            }
        }

        public ConfigClientBase(ConfigStoreBase configStore, ConfigSettingScope scope)
        {
            ConfigStore = configStore;
            Scope = scope;
        }

        public virtual Dictionary<string, string> GetAll()
        {
            Config config = ConfigStore.Read();

            IEnumerable<ConfigSetting> allSettings = config.Settings;

            IEnumerable<ConfigSetting> effective = EffectiveSettingsEvaluator.GetEffective(allSettings, Scope);

            Dictionary<string, string> result = _interpolator.Interpolate(effective);

            return result;
        }

        public virtual string Get(string key)
        {
            var all = GetAll();

            if (!all.ContainsKey(key))
            {
                return null;
            }

            return all[key];
        }
    }
}
