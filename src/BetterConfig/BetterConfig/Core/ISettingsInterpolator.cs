using System.Collections.Generic;

namespace BetterConfig.Core
{
    /// <summary>
    /// Allows to process raw config settings into the "evaluated" or "interpolated"
    /// key/value pairs. This process is needed to support references within config settings.
    /// </summary>
    public interface ISettingsInterpolator
    {
        Dictionary<string, string> Interpolate(IEnumerable<ConfigSetting> settings);
    }
}
