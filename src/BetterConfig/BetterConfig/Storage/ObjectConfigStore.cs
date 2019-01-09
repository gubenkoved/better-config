using System;

namespace BetterConfig.Storage
{
    public class ObjectConfigStore : ConfigStoreBase
    {
        private Config _config;
        private Func<Config> _func;

        public ObjectConfigStore(Config config)
        {
            _config = config;
        }

        public ObjectConfigStore(Func<Config> generator)
        {
            _func = generator;
        }

        public override Config Read()
        {
            return _config ?? _func();
        }

        public override void Save(Config config)
        {
            throw new NotImplementedException();
        }
    }
}
