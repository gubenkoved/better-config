using BetterConfig.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;

namespace BetterConfig.Server.Controllers
{
    [RoutePrefix("api/config")]
    public class ConfigController : ApiController
    {
        public static Config _config;

        public ConfigController()
        {
            if (_config == null)
            {
                string path = HostingEnvironment.MapPath(@"~/App_Data/example.json");

                var json = File.ReadAllText(path);

                var provider = new JsonConfigStore(json);

                _config = provider.Read();
            }
        }

        [Route("")]
        [HttpGet]
        public Config GetAll()
        {
            return _config;
        }

        [Route("interpolated")]
        [HttpGet]
        public IDictionary<string, string> GetAllInterpolated(
            string environment = null, string app = null, string appInstance = null)
        {
            var targetScope = ConfigSettingScope.Create(environment, app, appInstance);
            var ocs = new ObjectConfigStore(_config);
            var client = new ConfigClient(ocs, targetScope);

            return client.GetAll();
        }
    }
}
