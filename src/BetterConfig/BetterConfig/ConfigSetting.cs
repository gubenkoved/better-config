using System.Runtime.Serialization;

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
