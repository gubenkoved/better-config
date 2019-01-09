namespace BetterConfig.Storage
{
    public abstract class ConfigStoreBase
    {
        public abstract Config Read();

        public abstract void Save(Config config);
    }
}
