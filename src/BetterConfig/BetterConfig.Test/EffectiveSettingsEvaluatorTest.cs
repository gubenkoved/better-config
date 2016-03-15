using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterConfig.Test
{
    [TestClass]
    public class EffectiveSettingsEvaluatorTest
    {
        [TestMethod]
        public void GlobalSettingOnlyTest()
        {
            var global = new[]
            {
                new ConfigSetting("a", "1"),
                new ConfigSetting("b", "2"),
                new ConfigSetting("c", "3"),
            };

            var effective = EffectiveSettingsEvaluator.GetEffective(global, ConfigSettingScope.Global);

            var allDic = global.ToDictionary(x => x.Key, x => x.Definition);
            var effectiveDic = effective.ToDictionary(x => x.Key, x => x.Definition);

            CollectionAssert.AreEquivalent(allDic, effectiveDic);

        }

        [TestMethod]
        public void EnvSettingOverridesGlobalSettingsTest()
        {
            var envScope = ConfigSettingScope.Create("DEV");

            var all = new[]
            {
                new ConfigSetting("a", "1"),
                new ConfigSetting("b", "2"),
                new ConfigSetting("c", "3"),

                new ConfigSetting("a", "11")
                {
                    Scope = envScope
                },
            };

            var effectiveGlobal = EffectiveSettingsEvaluator.GetEffectiveAsDict(all, ConfigSettingScope.Global);
            var effectiveForEnv = EffectiveSettingsEvaluator.GetEffectiveAsDict(all, envScope);

            Assert.AreEqual("1", effectiveGlobal["a"].Definition);
            Assert.AreEqual("11", effectiveForEnv["a"].Definition);
        }

        [TestMethod]
        public void AppSettingOverridesGlobalAndEnvSettingsTest()
        {
            var envScope = ConfigSettingScope.Create("DEV");
            var appScope = ConfigSettingScope.Create("DEV", "APP1");

            var all = new[]
            {
                new ConfigSetting("a", "1"),
                new ConfigSetting("b", "2"),
                new ConfigSetting("c", "3"),

                new ConfigSetting("a", "11")
                {
                    Scope = envScope
                },

                new ConfigSetting("a", "111")
                {
                    Scope = appScope
                },
            };

            var effectiveGlobal = EffectiveSettingsEvaluator.GetEffectiveAsDict(all, ConfigSettingScope.Global);
            var effectiveForEnv = EffectiveSettingsEvaluator.GetEffectiveAsDict(all, envScope);
            var effectiveForApp = EffectiveSettingsEvaluator.GetEffectiveAsDict(all, appScope);

            Assert.AreEqual("1", effectiveGlobal["a"].Definition);
            Assert.AreEqual("11", effectiveForEnv["a"].Definition);
            Assert.AreEqual("111", effectiveForApp["a"].Definition);
        }

        [TestMethod]
        public void AppInstanceSettingOverridesGlobalAndEnvAndAppSettingsTest()
        {
            var envScope = ConfigSettingScope.Create("DEV");
            var appScope = ConfigSettingScope.Create("DEV", "APP1");
            var appInstScope = ConfigSettingScope.Create("DEV", "APP1", "I1");

            var all = new[]
            {
                new ConfigSetting("a", "1"),
                new ConfigSetting("b", "2"),
                new ConfigSetting("c", "3"),

                new ConfigSetting("a", "11")
                {
                    Scope = envScope
                },

                new ConfigSetting("a", "111")
                {
                    Scope = appScope
                },

                new ConfigSetting("a", "1111")
                {
                    Scope = appInstScope
                },
            };

            var effectiveGlobal = EffectiveSettingsEvaluator.GetEffectiveAsDict(all, ConfigSettingScope.Global);
            var effectiveForEnv = EffectiveSettingsEvaluator.GetEffectiveAsDict(all, envScope);
            var effectiveForApp = EffectiveSettingsEvaluator.GetEffectiveAsDict(all, appScope);
            var effectiveForAppInst = EffectiveSettingsEvaluator.GetEffectiveAsDict(all, appInstScope);

            Assert.AreEqual("1", effectiveGlobal["a"].Definition);
            Assert.AreEqual("11", effectiveForEnv["a"].Definition);
            Assert.AreEqual("111", effectiveForApp["a"].Definition);
            Assert.AreEqual("1111", effectiveForAppInst["a"].Definition);
        }

        [TestMethod]
        public void EnvironmentsDoNotInterferWithEachOtherTest()
        {
            var env1Scope = ConfigSettingScope.Create("DEV");
            var env2Scope = ConfigSettingScope.Create("QA");

            var all = new[]
            {
                new ConfigSetting("a", "1"),
                new ConfigSetting("b", "2"),
                new ConfigSetting("c", "3"),

                new ConfigSetting("a", "4")
                {
                    Scope = env1Scope
                },

                new ConfigSetting("a", "5")
                {
                    Scope = env2Scope
                },
            };

            var effectiveForEnv1 = EffectiveSettingsEvaluator.GetEffectiveAsDict(all, env1Scope);
            var effectiveForEnv2 = EffectiveSettingsEvaluator.GetEffectiveAsDict(all, env2Scope);

            Assert.AreEqual("4", effectiveForEnv1["a"].Definition);
            Assert.AreEqual("5", effectiveForEnv2["a"].Definition);
        }

        [TestMethod]
        public void AppOnlyBoundSettingTest()
        {
            var envScope = ConfigSettingScope.ForEnvironment("DEV");
            var appScope = ConfigSettingScope.ForApp("APP1");

            var all = new[]
            {
                new ConfigSetting("a", "1"),
                new ConfigSetting("b", "2"),
                new ConfigSetting("c", "3"),

                new ConfigSetting("a", "4")
                {
                    Scope = envScope,
                },

                new ConfigSetting("a", "5")
                {
                    Scope = appScope,
                },
            };

            var effectiveGlobal = EffectiveSettingsEvaluator.GetEffectiveAsDict(all, ConfigSettingScope.Global);
            var effectiveForEnv = EffectiveSettingsEvaluator.GetEffectiveAsDict(all, envScope);
            var effectiveForApp = EffectiveSettingsEvaluator.GetEffectiveAsDict(all, appScope);

            Assert.AreEqual("1", effectiveGlobal["a"].Definition);
            Assert.AreEqual("4", effectiveForEnv["a"].Definition);
            Assert.AreEqual("5", effectiveForApp["a"].Definition);
        }
    }
}
