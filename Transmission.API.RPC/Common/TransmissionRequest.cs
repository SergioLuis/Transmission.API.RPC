using System.Collections.Generic;

using Newtonsoft.Json;

using Transmission.API.RPC.ExtensionMethods;

namespace Transmission.API.RPC.Common
{
    /// <summary>
    /// Transmission request 
    /// </summary>
    public class TransmissionRequest : CommunicateBase
    {
        /// <summary>
        /// Name of the method to invoke
        /// </summary>
        [JsonProperty("method")]
        public string Method { get; set; }

        public TransmissionRequest(string method)
        {
            Method = method;
        }

        public void AddArgument(string key, object value)
        {
            if (Arguments == null)
                Arguments = new Dictionary<string, object>();

            Arguments.Add(key, value);
        }

        public void AddArguments(ArgumentsBase arguments)
        {
            Dictionary<string, object> args = arguments.ToDictionary();

            if (Arguments == null)
                Arguments = new Dictionary<string, object>();

            Arguments.AddRange(args);
        }
    }
}
