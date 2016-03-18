using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using BetterConfig.Storage;

namespace BetterConfig.Test
{
    [TestClass]
    public class ConfigStoreTest
    {
        [TestMethod]
        public void ConfigurationManagerConfigProvider()
        {
            var provider = new ConfigurationManagerConfigStore();

            var config = provider.Read();

            Assert.IsNotNull(config.Settings.SingleOrDefault(x => x.Key == "a"));
            Assert.IsNotNull(config.Settings.SingleOrDefault(x => x.Key == "b"));

            Assert.AreEqual("value", config.Settings.Single(x => x.Key == "a").Definition);
            Assert.AreEqual("42", config.Settings.Single(x => x.Key == "b").Definition);
        }

        [TestMethod]
        public void JsonConfigProvider()
        {
            var provider = new JsonConfigStore("./files/config.json");

            var config = provider.Read();

            Assert.AreEqual(4, config.Settings.Count());
            Assert.AreEqual(2, config.Settings.Where(x => x.Scope.Environment == "E2").Count());

            Assert.AreEqual("V4", config.Settings.Where(x => x.Scope.Environment == "E2")
                .FirstOrDefault(x => x.Key == "K2").Definition);
        }
    }
}
