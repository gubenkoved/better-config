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
        [TestClass]
        public class EscapingTests
        {
            private SettingsInterpolator _interpolator = new SettingsInterpolator();

            [TestMethod]
            public void EscapeTest()
            {
                Assert.AreEqual(@"x", _interpolator.Escape(@"x"));
                Assert.AreEqual(@"$x", _interpolator.Escape(@"$x"));
                Assert.AreEqual(@"\${x}", _interpolator.Escape(@"${x}"));
                Assert.AreEqual(@"\\\${x}", _interpolator.Escape(@"\${x}"));
                Assert.AreEqual(@"\\\\\${x}", _interpolator.Escape(@"\\${x}"));
                Assert.AreEqual(@"\\\\\\\${x}", _interpolator.Escape(@"\\\${x}"));

                // if there is no candidates to be an expression to evaluate - we should not escape anything
                Assert.AreEqual(@"do\not\e$cape{anything}here", _interpolator.Escape(@"do\not\e$cape{anything}here"));
                Assert.AreEqual(@"$ {x}", _interpolator.Escape(@"$ {x}"));

                // characters that not going to escape start of expression should not be escaped
                Assert.AreEqual(@"\something\${x}", _interpolator.Escape(@"\something${x}"));
            }

            [TestMethod]
            public void ShouldBeEvaluatedLiterallyTest()
            {
                var all = new[]
                {
                    new ConfigSetting("a", @"do\not\e$cape{anything}here"),
                };

                var actual = _interpolator.Interpolate(all);

                Assert.AreEqual(@"do\not\e$cape{anything}here", actual["a"]);
            }

            [TestMethod]
            public void EscapedReferenceTest()
            {
                var all = new[]
                {
                    new ConfigSetting("a", @"\${x}"),
                    new ConfigSetting("b", @"\\\${x}"),
                    new ConfigSetting("c", @"\\\\\${x}"),
                };

                var actual = _interpolator.Interpolate(all);

                Assert.AreEqual(@"${x}", actual["a"]);
                Assert.AreEqual(@"\${x}", actual["b"]);
                Assert.AreEqual(@"\\${x}", actual["c"]);
            }

            [TestMethod]
            public void EscapedEscapeCharReferenceTest()
            {
                var all = new[]
                {
                    new ConfigSetting("x", "x"),
                    new ConfigSetting("a", @"\\${x}"),
                    new ConfigSetting("b", @"\\\\${x}"),
                    new ConfigSetting("c", @"\\\\\\${x}"),
                };

                var actual = _interpolator.Interpolate(all);

                Assert.AreEqual(@"\x", actual["a"]);
                Assert.AreEqual(@"\\x", actual["b"]);
                Assert.AreEqual(@"\\\x", actual["c"]);
            }
        }

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

        [TestMethod]
        [ExpectedException(typeof(BetterConfigException))]
        public void SelfReferencesTest()
        {
            var all = new[]
            {
                new ConfigSetting("a", "${a}"),
            };

            _interpolator.Interpolate(all);
        }
    }
}
