using BetterConfig.Core;
using BetterConfig.Storage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterConfig.Test
{
    [TestClass]
    public class ConfigClientTest
    {
        [TestMethod]
        public void BasicTest()
        {
            var ocs = new ObjectConfigStore(() => new Config()
            {
                Settings = new[]
                {
                    new ConfigSetting("a", "1"),
                    new ConfigSetting("b", "2"),
                    new ConfigSetting("c", "3"),
                }
            });

            var client = new ConfigClient(ocs, ConfigSettingScope.Global);

            Assert.AreEqual(3, client.GetAll().Count);
            Assert.AreEqual("1", client.Get("a"));
            Assert.AreEqual("2", client.Get("b"));
            Assert.AreEqual("3", client.Get("c"));
        }

        [TestMethod]
        public void BasicScopingTest()
        {
            var ocs = new ObjectConfigStore(() => new Config()
            {
                Settings = new[]
                {
                    new ConfigSetting("a", "1"),
                    new ConfigSetting("b", "2")
                    {
                        Scope = ConfigSettingScope.ForApp("X")
                    },
                    new ConfigSetting("b", "3")
                    {
                        Scope = ConfigSettingScope.ForApp("Y")
                    },
                }
            });

            #region Global
            var globalClient = new ConfigClient(ocs, ConfigSettingScope.Global);

            Assert.AreEqual(1, globalClient.GetAll().Count);
            Assert.AreEqual("1", globalClient.Get("a"));
            #endregion

            #region App X
            var appXClient = new ConfigClient(ocs, ConfigSettingScope.ForApp("X"));

            Assert.AreEqual(2, appXClient.GetAll().Count);
            Assert.AreEqual("1", appXClient.Get("a"));
            Assert.AreEqual("2", appXClient.Get("b"));
            #endregion

            #region App Y
            var appYClient = new ConfigClient(ocs, ConfigSettingScope.ForApp("Y"));

            Assert.AreEqual(2, appYClient.GetAll().Count);
            Assert.AreEqual("1", appYClient.Get("a"));
            Assert.AreEqual("3", appYClient.Get("b")); 
            #endregion
        }

        [TestMethod]
        public void BasicReferencesTest()
        {
            var ocs = new ObjectConfigStore(() => new Config()
            {
                Settings = new[]
                {
                    new ConfigSetting("a", "1"),
                    new ConfigSetting("b", "2${a}")
                }
            });

            var globalClient = new ConfigClient(ocs, ConfigSettingScope.Global);

            Assert.AreEqual("21", globalClient.Get("b"));
        }

        [TestMethod]
        public void MultiLevelReferencesTest()
        {
            var ocs = new ObjectConfigStore(() => new Config()
            {
                Settings = new[]
                {
                    new ConfigSetting("a", "1"),
                    new ConfigSetting("b", "2${a}"),
                    new ConfigSetting("c", "${a}+${b}"),
                }
            });

            var globalClient = new ConfigClient(ocs, ConfigSettingScope.Global);

            Assert.AreEqual("1+21", globalClient.Get("c"));
        }
    }
}
