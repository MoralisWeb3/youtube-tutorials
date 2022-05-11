/**
 *           Module: UnityNewtonsoftSerializer.cs
 *  Descriptiontion: Class that wraps json serialization routione. Since Unity supports
 *                   multiple versions of Newtonsoft this class wraps the Newtonsoft library
 *                   installed in this Unity instance so that it can be passed to and used
 *                   internally in Moralis.
 *           Author: Moralis Web3 Technology AB, 559307-5988 - David B. Goodrich
 *  
 *  MIT License
 *  
 *  Copyright (c) 2021 Moralis Web3 Technology AB, 559307-5988
 *  
 *  Permission is hereby granted, free of charge, to any person obtaining a copy
 *  of this software and associated documentation files (the "Software"), to deal
 *  in the Software without restriction, including without limitation the rights
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *  copies of the Software, and to permit persons to whom the Software is
 *  furnished to do so, subject to the following conditions:
 *  
 *  The above copyright notice and this permission notice shall be included in all
 *  copies or substantial portions of the Software.
 *  
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *  SOFTWARE.
 */
using System.Collections.Generic;
using Newtonsoft.Json;

#if UNITY_WEBGL
using Moralis.WebGL.Platform.Abstractions;

namespace Assets.Scripts
{
    /// <summary>
    /// Defines a Newtonsoft wrapper around the Unity specific Newtonsoft library so that is
    /// can be passed into Moralis.
    /// </summary>
    public class UnityNewtosoftSerializer : IJsonSerializer
    {
        public IDictionary<string, object> DefaultOptions { get; set; }

        public UnityNewtosoftSerializer()
        {
            DefaultOptions = new Dictionary<string, object>();
            DefaultOptions.Add("NullValueHandling", NullValueHandling.Ignore);
            DefaultOptions.Add("ReferenceLoopHandling", ReferenceLoopHandling.Serialize);
            DefaultOptions.Add("DateFormatString", "yyyy-MM-ddTHH:mm:ss.fffZ");
        }

        public T Deserialize<T>(string json, IDictionary<string, object> options = null)
        {
            if (options is { })
            {
                return JsonConvert.DeserializeObject<T>(json, OptionsToSettings(options));
            }
            else
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
        }

        public string Serialize(object target, IDictionary<string, object> options = null)
        {
            if (options is { })
            {
                return JsonConvert.SerializeObject(target, OptionsToSettings(options));
            }
            else
            {
                return JsonConvert.SerializeObject(target);
            }
        }

        private JsonSerializerSettings OptionsToSettings(IDictionary<string, object> options)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                DateFormatString = options.ContainsKey("DateFormatString") ? (string)options["DateFormatString"] : null,
                NullValueHandling = options.ContainsKey("NullValueHandling") ? (NullValueHandling)options["NullValueHandling"] : NullValueHandling.Ignore,
                ReferenceLoopHandling = options.ContainsKey("ReferenceLoopHandling") ? (ReferenceLoopHandling)options["ReferenceLoopHandling"] : ReferenceLoopHandling.Ignore
            };

            return settings;
        }
    }
}
#else
using Moralis.Platform.Abstractions;

namespace Assets.Scripts
{
    /// <summary>
    /// Defines a Newtonsoft wrapper around the Unity specific Newtonsoft library so that is
    /// can be passed into Moralis.
    /// </summary>
    public class UnityNewtosoftSerializer : IJsonSerializer
    {
        public IDictionary<string, object> DefaultOptions { get; set; }

        public UnityNewtosoftSerializer()
        {
            DefaultOptions = new Dictionary<string, object>();
            DefaultOptions.Add("NullValueHandling", NullValueHandling.Ignore);
            DefaultOptions.Add("ReferenceLoopHandling", ReferenceLoopHandling.Serialize);
            DefaultOptions.Add("DateFormatString", "yyyy-MM-ddTHH:mm:ss.fffZ");
        }

        public T Deserialize<T>(string json, IDictionary<string, object> options = null)
        {
            if (options is { })
            {
                return JsonConvert.DeserializeObject<T>(json, OptionsToSettings(options));
            }
            else
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
        }

        public string Serialize(object target, IDictionary<string, object> options = null)
        {
            if (options is { })
            {
                return JsonConvert.SerializeObject(target, OptionsToSettings(options));
            }
            else
            {
                return JsonConvert.SerializeObject(target);
            }
        }

        private JsonSerializerSettings OptionsToSettings(IDictionary<string, object> options)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                DateFormatString = options.ContainsKey("DateFormatString") ? (string)options["DateFormatString"] : null,
                NullValueHandling = options.ContainsKey("NullValueHandling") ? (NullValueHandling)options["NullValueHandling"] : NullValueHandling.Ignore,
                ReferenceLoopHandling = options.ContainsKey("ReferenceLoopHandling") ? (ReferenceLoopHandling)options["ReferenceLoopHandling"] : ReferenceLoopHandling.Ignore
            };

            return settings;
        }
    }
}
#endif
