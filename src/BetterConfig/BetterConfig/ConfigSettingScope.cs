using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterConfig
{
    public struct ConfigSettingScope
    {
        public string Environment { get; set; }
        public string Application { get; set; }
        public string ApplicationInstance { get; set; }
    }
}
