using BetterConfig.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BetterConfig
{
    /// <summary>
    /// Interpolates setting definition to the value
    /// </summary>
    public class SettingsInterpolator : ISettingsInterpolator
    {
        private Regex _referenceFragmentRegex;

        public SettingsInterpolator()
        {
            _referenceFragmentRegex = new Regex(@"\$\{(?<ref>[a-z_-]*)\}",
                RegexOptions.Compiled
                | RegexOptions.CultureInvariant
                | RegexOptions.IgnoreCase);
        }

        public Dictionary<string, string> Interpolate(IEnumerable<ConfigSetting> settings)
        {
            if (settings.Select(x => x.Key).Distinct().Count() != settings.Count())
            {
                throw new BetterConfigException("Settings set must not contain items with same keys");
            }

            var evaluated = new Dictionary<string, string>();

            var toEvaluate = new List<ConfigSetting>(settings);

            while(toEvaluate.Any())
            {
                int toEvalBefore = toEvaluate.Count;

                for (int i = toEvaluate.Count - 1; i >= 0; i--)
                {
                    var item = toEvaluate[i];

                    var refMatches = _referenceFragmentRegex.Matches(item.Definition);

                    #region Case for Definitions with cross references
                    if (refMatches.Count > 0) // definition contains references
                    {
                        Match[] refValueMatches = refMatches.OfType<Match>().ToArray();

                        string[] refValues = refValueMatches
                            .Select(m => m.Groups["ref"].Value)
                            .ToArray();

                        // check that all references can be resolved then resolve, otherwise skip for now
                        if (refValues.All(x => evaluated.ContainsKey(x)))
                        {
                            string evaluationResult = item.Definition;

                            foreach (var refMatch in refValueMatches)
                            {
                                var refKey = refMatch.Groups["ref"].Value;

                                if (refKey.Length == 0)
                                {
                                    throw new BetterConfigException("Reference is malformed, because referenced key is empty");
                                }

                                evaluationResult = evaluationResult.Replace(refMatch.Value, evaluated[refKey]);
                            }

                            evaluated[item.Key] = evaluationResult;
                            toEvaluate.RemoveAt(i);
                        }
                    }
                    #endregion
                    #region Plain values - no cross references case
                    else // no references -> count as evaluated
                    {
                        evaluated[item.Key] = item.Definition;
                        toEvaluate.RemoveAt(i);
                    } 
                    #endregion
                }

                if (toEvaluate.Count == toEvalBefore)
                {
                    throw new BetterConfigException($"Unable to evaluate those settings: {string.Join(", ", toEvaluate.Select(x => x.Key))}");
                }
            }

            return evaluated;
        }
    }
}
