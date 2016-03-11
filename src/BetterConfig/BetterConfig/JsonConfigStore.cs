using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterConfig
{
    public class JsonConfigStore : ConfigStoreBase
    {
        public const string RootSettingsArrayKey = "settings";
        public const string SettingKeyKey = "key";
        public const string SettingValueKey = "value";
        public const string SettingScopeEnvironmentKey = "environment";
        public const string SettingScopeAppKey = "app";
        public const string SettingScopeAppInstanceKey = "appInstance";

        public string JsonText { get; private set; }

        public JsonConfigStore(string json)
        {
            JsonText = json;
        }

        public override IEnumerable<ConfigSetting> ReadAll()
        {
            var result = new List<ConfigSetting>();

            var root = JObject.Parse(JsonText);

            var data = root.Value<JArray>(RootSettingsArrayKey);

            if (data == null)
            {
                throw new ConfigurationErrorsException($"Can NOT find root object element with settings: '{RootSettingsArrayKey}'");
            }

            foreach (var item in data)
            {
                string iKey = item.Value<string>(SettingKeyKey);
                string iVal = item.Value<string>(SettingValueKey);

                if (iKey == null)
                {
                    throw new ConfigurationErrorsException($"Can NOT find setting key on following element: {item.ToString()}");
                }

                if (iVal == null)
                {
                    throw new ConfigurationErrorsException($"Can NOT find setting value on following element: {item.ToString()}");
                }

                if (iVal != null) // simple syntax case
                {
                    result.Add(new ConfigSetting(iKey, iVal)
                    {
                        Scope = new ConfigSettingScope()
                        {
                            Environment = item.Value<string>(SettingScopeEnvironmentKey),
                            Application = item.Value<string>(SettingScopeAppKey),
                            ApplicationInstance = item.Value<string>(SettingScopeAppInstanceKey),
                        },
                    });
                }
            }

            return result;
        }
    }
}
