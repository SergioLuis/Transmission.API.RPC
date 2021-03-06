﻿using System.Collections.Generic;

using Newtonsoft.Json;

namespace Transmission.API.RPC.Common
{
    public abstract class CommunicateBase
    {
        /// <summary>
        /// Data
        /// </summary>
        [JsonProperty("arguments")]
        public Dictionary<string, object> Arguments { get; set; }

        /// <summary>
        /// Number (id)
        /// </summary>
        [JsonProperty("tag")]
        public int Tag { get; set; }

        /// <summary>
        /// Convert to JSON string
        /// </summary>
        /// <returns></returns>
        public virtual string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Deserialize to class
        /// </summary>
        /// <returns></returns>
        public T Deserialize<T>()
        {
            var argumentsString = JsonConvert.SerializeObject(this.Arguments);
            return JsonConvert.DeserializeObject<T>(argumentsString);
        }
    }
}
