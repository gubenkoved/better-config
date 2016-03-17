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
        public string Application { get; set; }

        [DataMember]
        public string ApplicationInstance { get; set; }

        public bool IsEnvironmentBound
        {
            get
            {
                return Environment != null;
            }
        }
        public bool IsApplicationBound
        {
            get
            {
                return Application != null;
            }
        }
        public bool IsApplicationInstanceBound
        {
            get
            {
                return ApplicationInstance != null;
            }
        }

        public bool IsGlobal
        {
            get
            {
                return !IsEnvironmentBound
                    && !IsApplicationBound
                    && !IsApplicationInstanceBound;
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
                    + (IsApplicationBound ? 1 : 0) << 1
                    + (IsApplicationInstanceBound ? 1 : 0) << 2;
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
                && (!IsApplicationBound || Application == anotherScope.Application)
                && (!IsApplicationInstanceBound || ApplicationInstance == anotherScope.ApplicationInstance);
        }

        public static ConfigSettingScope Create(string environment, string app = null, string appInstance = null)
        {
            return new ConfigSettingScope()
            {
                Environment = environment,
                Application = app,
                ApplicationInstance = appInstance,
            };
        }

        public static ConfigSettingScope ForApp(string app)
        {
            return new ConfigSettingScope()
            {
                Application = app,
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
