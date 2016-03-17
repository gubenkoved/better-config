using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BetterConfig
{
    [DataContract]
    public class ConfigSetting
    {
        [DataMember]
        public virtual string Key { get; set; }

        [DataMember]
        public virtual string Definition { get; set; }

        [DataMember]
        public virtual ConfigSettingScope Scope { get; set; }

        public ConfigSetting(string key, string definition)
        {
            Key = key;
            Definition = definition;
        }
    }
}
