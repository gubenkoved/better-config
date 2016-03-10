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

            Assert.AreEqual("value", config.Single(x => x.Key == "a").Value.Value);
            Assert.AreEqual("42", config.Single(x => x.Key == "b").Value.Value);
        }

        [TestMethod]
        public void JsonConfigProvider()
        {
            var json = File.ReadAllText("./files/config.json");

            var provider = new JsonConfigStore(json);

            var config = provider.ReadAll();

            Assert.AreEqual(3, config.Count());
            Assert.AreEqual(2, config.Where(x => x.Environemnt == "test").Count());

            Assert.IsNotNull(config.Where(x => x.Environemnt == "test").FirstOrDefault(x => x.Key == "plain"));
            Assert.IsNotNull(config.Where(x => x.Environemnt == "test").FirstOrDefault(x => x.Key == "advanced"));

            Assert.AreEqual("42", config.Where(x => x.Environemnt == "test").FirstOrDefault(x => x.Key == "plain").Value.Value);
        }
    }
}
