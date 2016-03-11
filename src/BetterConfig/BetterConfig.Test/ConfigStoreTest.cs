using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace BetterConfig.Test
{
    [TestClass]
    public class ConfigStoreTest
    {
        [TestMethod]
        public void ConfigurationManagerConfigProvider()
        {
            var provider = new ConfigurationManagerConfigStore();

            var config = provider.ReadAll();

            Assert.IsNotNull(config.SingleOrDefault(x => x.Key == "a"));
            Assert.IsNotNull(config.SingleOrDefault(x => x.Key == "b"));

            Assert.AreEqual("value", config.Single(x => x.Key == "a").Definition);
            Assert.AreEqual("42", config.Single(x => x.Key == "b").Definition);
        }

        [TestMethod]
        public void JsonConfigProvider()
        {
            var json = File.ReadAllText("./files/config.json");

            var provider = new JsonConfigStore(json);

            var config = provider.ReadAll();

            Assert.AreEqual(4, config.Count());
            Assert.AreEqual(2, config.Where(x => x.Scope.Environment == "E2").Count());

            Assert.AreEqual("V4", config.Where(x => x.Scope.Environment == "E2").FirstOrDefault(x => x.Key == "K2").Definition);
        }
    }
}
