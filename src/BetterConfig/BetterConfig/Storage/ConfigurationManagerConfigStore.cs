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
        public override Config Read()
        {
            var settings = new List<ConfigSetting>();

            foreach (var key in ConfigurationManager.AppSettings.AllKeys)
            {
                settings.Add(new ConfigSetting(key, ConfigurationManager.AppSettings[key]));
            }

            return new Config()
            {
                Settings = settings,
            };
        }

        public override void Save(Config config)
        {
            throw new NotImplementedException();
        }
    }
}
