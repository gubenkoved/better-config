using BetterConfig.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterConfig.Storage
{
    public class ObjectConfigStore : ConfigStoreBase
    {
        private IEnumerable<ConfigSetting> _settings;
        private Func<IEnumerable<ConfigSetting>> _func;

        public ObjectConfigStore(IEnumerable<ConfigSetting> settings)
        {
            _settings = settings;
        }

        public ObjectConfigStore(Func<IEnumerable<ConfigSetting>> generator)
        {
            _func = generator;
        }

        public override IEnumerable<ConfigSetting> ReadAll()
        {
            return _settings ?? _func();
        }
    }
}
