﻿* Need to decouple core from the persistence all together (e.g. get rid of DataContract attributes)

* May be there is a sense in simplifying things and getting rid of granular scope levels like environment/app/etc and just sticking to some generic "tags" (however, there might be need in tag priorities -- is it server or client concern by the way?)

* Current implementation does a *terrible* (!) job separating client concerns from server concerns: For instance client does not care how do we store and interpolate stuff; Make API super easy for Clients, make it client centric;
  Basically, in order to use configuration client just need to initialize (probably optionally with reasonable defaults) scopes/tags/whatever and it should be all set to get configuration values (of different types btw) by keys:

```CSharp
  BetterConfig.Initialize(...);
  var x = BetterConfig.Get<string>("AzureSqlConnectionString");
  var y = BetterConfig.Get<int>("CacheTtlSeconds");
```

* Change notifications would be nice to have, if it will not make it too messy

* Make it simple to run application locally w/o any external dependencies -- for instance providing JSON/XML configuration

* Need to decide whether interpolation (expanding references between config variables) is server or client concept... This decisions has some implications, for instance, when interpolation is client concept then client will be forced to get multiple config values during a request of the a single one because of the possible cross refereneces

# Summary

It feels that it's better to start from scratch if I ever get incentive to continue working on this thing.

# Useful links

https://docs.microsoft.com/en-us/azure/architecture/patterns/external-configuration-store