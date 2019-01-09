using System.Collections.Generic;

namespace BetterConfig
{
    public class Config
    {
        public IEnumerable<ConfigSetting> Settings { get; set; } = new List<ConfigSetting>();
    }
}
