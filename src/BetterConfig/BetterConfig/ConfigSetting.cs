using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterConfig
{
    public class ConfigSetting
    {
        public virtual string Key { get; set; }

        public virtual string Definition { get; set; }

        public virtual ConfigSettingScope Scope { get; set; }

        public ConfigSetting(string key, string definition)
        {
            Key = key;
            Definition = definition;
        }
    }
}
