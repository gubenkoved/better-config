using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterConfig
{
    public abstract class ConfigStoreBase
    {
        public abstract IEnumerable<ConfigSetting> ReadAll();
    }
}
