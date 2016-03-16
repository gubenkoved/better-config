using BetterConfig.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private struct CharAnnotation
        {
            public bool IsRefStart;
            public bool IsEscape;
            public bool IsEscaped;
        }

        private static readonly Regex _referenceFragmentRegex;
        private static readonly char _escapeChar = '\\';
        private static readonly char[] _specialChars = new[] { '$' };

        static SettingsInterpolator()
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
                    ConfigSetting       configItem   = toEvaluate[i];
                    string              def          = configItem.Definition;
                    CharAnnotation[]    defAnnotated = Annotate(configItem.Definition);
                    string              defWithoutMetachars
                        = new string(def.Where((c, ci) => !defAnnotated[ci].IsEscape).ToArray());

                    var refMatches = _referenceFragmentRegex
                        .Matches(def)
                        .OfType<Match>()
                        // remove matches that are false-positives due to escaping
                        .Where(m => defAnnotated[m.Index].IsRefStart && !defAnnotated[m.Index].IsEscaped)
                        .ToArray();

                    #region Case for Definitions with cross references
                    if (refMatches.Length > 0) // definition contains references
                    {
                        string[] refValues = refMatches
                            .Select(m => m.Groups["ref"].Value)
                            .ToArray();

                        // check that all references can be resolved then resolve, otherwise skip for now
                        if (refValues.All(x => evaluated.ContainsKey(x)))
                        {
                            string evaluationResult = defWithoutMetachars;

                            foreach (var refMatch in refMatches)
                            {
                                var refKey = refMatch.Groups["ref"].Value;

                                if (refKey.Length == 0)
                                {
                                    throw new BetterConfigException("Reference is malformed, because referenced key is empty");
                                }

                                evaluationResult = evaluationResult.Replace(refMatch.Value, evaluated[refKey]);
                            }

                            evaluated[configItem.Key] = evaluationResult;
                            toEvaluate.RemoveAt(i);
                        }
                    }
                    #endregion
                    #region Plain values - no cross references case
                    else // no references -> count as evaluated
                    {
                        evaluated[configItem.Key] = defWithoutMetachars;
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

        // escapes raw setting definition in order to avoid special meaning of constructs
        // e.g. to treat "${a}" literally, not like reference syntax
        public string Escape(string raw)
        {
            var result = new List<char>();

            CharAnnotation[] annotation = Annotate(raw);

            for (int i = 0; i < raw.Length; i++)
            {
                if (annotation[i].IsEscape || annotation[i].IsEscaped || annotation[i].IsRefStart)
                {
                    result.Add(_escapeChar);
                }

                result.Add(raw[i]);
            }
            
            return new string(result.ToArray());
        }

        #region Private methods
        private CharAnnotation[] Annotate(string raw)
        {
            CharAnnotation[] annotation = new CharAnnotation[raw.Length];

            #region Search candidates to be expression
            var refMatches = _referenceFragmentRegex
                    .Matches(raw)
                    .OfType<Match>();

            foreach (var refMatch in refMatches)
            {
                annotation[refMatch.Index].IsRefStart = true;

                #region For each candidate figure out sequence of escape chars (if any) before
                if (refMatch.Index > 0)
                {
                    int escapeSequenceStartIdx = refMatch.Index - 1;

                    while (raw[escapeSequenceStartIdx] == _escapeChar)
                    {
                        // reached start of string
                        if (escapeSequenceStartIdx == 0) break;

                        escapeSequenceStartIdx -= 1;
                    }

                    // go forward one char if current symbol is not escape char - it's condition to exit from loop
                    // but not always be true due to possibility to reaching start of string
                    if (raw[escapeSequenceStartIdx] != _escapeChar)
                    {
                        escapeSequenceStartIdx += 1;
                    }

                    for (int i = escapeSequenceStartIdx; i < refMatch.Index; i += 2)
                    {
                        annotation[i].IsEscape = true;
                        annotation[i + 1].IsEscaped = true;
                    }
                }
                #endregion
            }
            #endregion


            return annotation;
        }
        #endregion
    }
}
