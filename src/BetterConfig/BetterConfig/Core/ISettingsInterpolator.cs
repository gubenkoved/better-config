using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterConfig.Core
{
    public interface ISettingsInterpolator
    {
        Dictionary<string, string> Interpolate(IEnumerable<ConfigSetting> settings);
    }
}
