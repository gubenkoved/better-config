using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterConfig
{
    public class Config
    {
        public IEnumerable<ConfigSetting> Settings { get; set; } = new List<ConfigSetting>();
    }
}
