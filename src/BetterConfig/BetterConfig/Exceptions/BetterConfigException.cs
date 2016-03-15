using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterConfig.Exceptions
{
    [Serializable]
    public class BetterConfigException : Exception
    {
        public BetterConfigException() { }
        public BetterConfigException(string message) : base(message) { }
        public BetterConfigException(string message, Exception inner) : base(message, inner) { }

        protected BetterConfigException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context)
        { }
    }
}
