using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterConfig
{
    public class ConfigClient : ConfigClientBase
    {
        public ConfigClient(ConfigStoreBase configStore)
            :base(configStore)
        {
        }
    }
}
