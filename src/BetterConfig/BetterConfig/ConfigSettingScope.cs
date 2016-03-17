using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BetterConfig
{
    [DataContract]
    public struct ConfigSettingScope
    {
        [DataMember]
        public string Environment { get; set; }

        [DataMember]
        public string App { get; set; }

        [DataMember]
        public string AppInstance { get; set; }

        public bool IsEnvironmentBound
        {
            get
            {
                return Environment != null;
            }
        }
        public bool IsAppBound
        {
            get
            {
                return App != null;
            }
        }
        public bool IsAppInstanceBound
        {
            get
            {
                return AppInstance != null;
            }
        }

        public bool IsGlobal
        {
            get
            {
                return !IsEnvironmentBound
                    && !IsAppBound
                    && !IsAppInstanceBound;
            }
        }

        /// <summary>
        /// Priority of scopes determines who overrides who in case of
        /// same Setting Key. Settings with bigger priority wins.
        /// </summary>
        internal int Priority
        {
            get
            {
                return (IsEnvironmentBound ? 1 : 0)
                    + (IsAppBound ? 1 : 0) << 1
                    + (IsAppInstanceBound ? 1 : 0) << 2;
            }
        }

        public static ConfigSettingScope Global
        {
            get
            {
                return new ConfigSettingScope();
            }
        }

        /// <summary>
        /// Returns true is values from this scopes can be applied for specific another scope.
        /// Generally true says that this scope is less restrictive than passed in scope.
        /// For instance,
        ///     (STAGE, null, null) applicable to (STATE, TST_APP, null), but not otherwise;
        ///     (null, A, null) applicable to (null, A, INST_1), but not otherwise;
        ///     (DEV, null, null) not applicable to (QA, null, null) and the opposite;
        /// </summary>
        internal bool IsApplicableTo(ConfigSettingScope anotherScope)
        {
            // alg:
            // for x in (Environment, App, AppInstance) in order check that
            // x is null or the same with passed in scope
            //
            // if at least one condition is not met, then return false
            // otherwise true;

            return (!IsEnvironmentBound || Environment == anotherScope.Environment)
                && (!IsAppBound || App == anotherScope.App)
                && (!IsAppInstanceBound || AppInstance == anotherScope.AppInstance);
        }

        /// <summary>
        /// Calculates "intersection" between scopes. If consider scope as set of filters and restrictions
        /// then "intersection" of scopes is equally or more restrictive filter that is subset of both.
        /// Lack of "intersection" means that conditions/filters are not compatible.
        /// 
        /// Example:
        ///     (DEV, null, null) intersection (null, APP1, null)   = (DEV, APP1, null)
        ///     (DEV, null, null) intersection (QA, null, null)     = null
        ///     (DEV, APP1, null) intersection (null, APP1, null)   = (DEV, APP1, null)
        /// </summary>
        public static ConfigSettingScope? Intersect(ConfigSettingScope s1, ConfigSettingScope s2)
        {
            if (s1.IsEnvironmentBound && s2.IsEnvironmentBound && s1.Environment != s2.Environment
                || s1.IsAppBound && s2.IsAppBound && s1.App != s2.App
                || s1.IsAppInstanceBound && s2.IsAppInstanceBound && s1.AppInstance != s2.AppInstance)
            {
                return null;
            }

            return new ConfigSettingScope()
            {
                Environment = s1.Environment ?? s2.Environment,
                App = s1.App ?? s2.App,
                AppInstance = s1.AppInstance ?? s2.AppInstance,
            };
        }

        public static ConfigSettingScope Create(string environment, string app = null, string appInstance = null)
        {
            return new ConfigSettingScope()
            {
                Environment = environment,
                App = app,
                AppInstance = appInstance,
            };
        }

        public static ConfigSettingScope ForApp(string app)
        {
            return new ConfigSettingScope()
            {
                App = app,
            };
        }

        public static ConfigSettingScope ForEnvironment(string environment)
        {
            return new ConfigSettingScope()
            {
                Environment = environment,
            };
        }
    }
}
