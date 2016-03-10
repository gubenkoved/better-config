using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterConfig
{
    public class ConfigSetting
    {
        public virtual string Environemnt { get; set; }

        public virtual string Key { get; set; }

        public virtual IEnumerable<ConfigValue> Values { get; set; }

        public ConfigValue Value
        {
            get
            {
                if (Values.Count() > 1)
                {
                    throw new InvalidOperationException();
                } else if (Values.Count() == 0)
                {
                    return null;
                }

                return Values.Single();
            }
            set
            {
                Values = new[] { value };
            }
        }

        public ConfigSetting(string key, IEnumerable<ConfigValue> values)
        {
            Key = key;
            Values = values;
        }

        public ConfigSetting(string key, string value)
        {
            Key = key;
            Values = new[]
            {
                new ConfigValue(value),
            };
        }
    }
}
