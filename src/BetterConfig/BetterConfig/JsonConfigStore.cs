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
        public string JsonText { get; private set; }

        public JsonConfigStore(string json)
        {
            JsonText = json;
        }

        public override IEnumerable<ConfigSetting> ReadAll()
        {
            var result = new List<ConfigSetting>();

            var root = JObject.Parse(JsonText);

            var data = root.Value<JArray>("data");

            foreach (var item in data)
            {
                string itemEnv = item.Value<string>("environment");
                string itemKey = item.Value<string>("key");
                string itemVal = item.Value<string>("value");

                if (itemVal != null) // simple syntax case
                {
                    result.Add(new ConfigSetting(itemKey, itemVal)
                    {
                        Environemnt = itemEnv,
                    });
                } else // case with extended syntax
                {
                    var itemValues = item.Value<JArray>("values")
                        .Select(jToken => new ConfigValue(jToken.Value<string>("value"))
                        {
                            EffectiveSinceUtc = Helper.FromEpoch(jToken.Value<int>("effective-since")),
                        }).ToList();

                    result.Add(new ConfigSetting(itemKey, itemValues)
                    {
                        Environemnt = itemEnv,
                    });
                }
            }

            return result;
        }
    }
}
