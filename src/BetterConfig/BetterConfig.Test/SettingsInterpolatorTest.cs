using BetterConfig.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterConfig.Test
{
    [TestClass]
    public class SettingsInterpolatorTest
    {
        private SettingsInterpolator _interpolator = new SettingsInterpolator();

        [TestMethod]
        public void BasicReferenceTest()
        {
            var all = new[]
            {
                new ConfigSetting("a", "1"),
                new ConfigSetting("b", "b${a}"),
            };

            var actual = _interpolator.Interpolate(all);

            var expected = new Dictionary<string, string>()
            {
                { "a", "1" },
                { "b", "b1" },
            };

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        public void MultiLevelReferenceTest()
        {
            var all = new[]
            {
                new ConfigSetting("a", "1"),
                new ConfigSetting("b", "b${a}"),
                new ConfigSetting("c", "c${b}"),
            };

            var actual = _interpolator.Interpolate(all);

            var expected = new Dictionary<string, string>()
            {
                { "a", "1" },
                { "b", "b1" },
                { "c", "cb1" },
            };

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        public void MultiReferencesInSingleSettingTest()
        {
            var all = new[]
            {
                new ConfigSetting("a", "a"),
                new ConfigSetting("b", "b"),
                new ConfigSetting("c", "${a}+${b}"),
            };

            var actual = _interpolator.Interpolate(all);

            var expected = new Dictionary<string, string>()
            {
                { "a", "a" },
                { "b", "b" },
                { "c", "a+b" },
            };

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(BetterConfigException))]
        public void CircularReferencesTest()
        {
            var all = new[]
            {
                new ConfigSetting("a", "${b}"),
                new ConfigSetting("b", "${a}"),
            };

            _interpolator.Interpolate(all);
        }
    }
}
