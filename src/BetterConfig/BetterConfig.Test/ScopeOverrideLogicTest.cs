using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterConfig.Test
{
    [TestClass]
    public class ScopeOverrideLogicTest
    {
        [TestMethod]
        public void GlobalIsApplicableToEverythingTest()
        {
            ConfigSettingScope global = ConfigSettingScope.Global;

            Assert.IsTrue(global.IsApplicableTo(ConfigSettingScope.Global));
            Assert.IsTrue(global.IsApplicableTo(ConfigSettingScope.Create("DEV")));
            Assert.IsTrue(global.IsApplicableTo(ConfigSettingScope.Create("DEV", "APP1")));
            Assert.IsTrue(global.IsApplicableTo(ConfigSettingScope.Create("DEV", "APP1", "INST1")));
        }

        [TestMethod]
        public void DifferentAppScopesNotApplicableToEachOtherTest()
        {
            var app1Scope = ConfigSettingScope.Create("DEV", "APP1");
            var app2Scope = ConfigSettingScope.Create("DEV", "APP2");

            Assert.IsFalse(app1Scope.IsApplicableTo(app2Scope));
            Assert.IsFalse(app2Scope.IsApplicableTo(app1Scope));
            
            // the same with unspecified env
            app1Scope = ConfigSettingScope.Create(null, "APP1");
            app2Scope = ConfigSettingScope.Create(null, "APP2");

            Assert.IsFalse(app1Scope.IsApplicableTo(app2Scope));
            Assert.IsFalse(app2Scope.IsApplicableTo(app1Scope));
        }

        [TestMethod]
        public void AppBoundNotApplicableToEnvBoundAndTheOppositeTest()
        {
            var envBound = ConfigSettingScope.Create("DEV", null);
            var appBound = ConfigSettingScope.Create(null, "APP2");

            Assert.IsFalse(envBound.IsApplicableTo(appBound));
            Assert.IsFalse(appBound.IsApplicableTo(envBound));
        }

        [TestMethod]
        public void EnvBoundApplicableToBothEnvAndAppBoundTest()
        {
            var envBound = ConfigSettingScope.Create("DEV", null);
            var envAppBound = ConfigSettingScope.Create("DEV", "APP2");

            Assert.IsTrue(envBound.IsApplicableTo(envAppBound));
            Assert.IsFalse(envAppBound.IsApplicableTo(envBound));
        }
    }
}
