#region License

// <copyright file="RestProxyTests.cs" company="SineSignal, LLC.">
//   Copyright 2007-2009 SineSignal, LLC.
//       Licensed under the Apache License, Version 2.0 (the "License");
//       you may not use this file except in compliance with the License.
//       A copy of the License can be found in the LICENSE file or you may 
//       obtain a copy of the License at
//
//           http://www.apache.org/licenses/LICENSE-2.0
//
//       Unless required by applicable law or agreed to in writing, software
//       distributed under the License is distributed on an "AS IS" BASIS,
//       WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//       See the License for the specific language governing permissions and
//       limitations under the License.
// </copyright>

#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace SineSignal.Ottoman.Generators
{
    /// <summary>
    /// Used to generate document IDs seeded with a random value from a CouchDB then incremented on the client.
    /// </summary>
    class HiLoGenerator : IGenerator<string>
    {
        public Dictionary<string, object> Options { get; set; }

        private string _uuid = null;
        private int _sequence = 0;

        /// <summary>
        /// Generates a unique document ID.
        /// </summary>
        /// <returns>A unique string each time the function is called.</returns>
        public string Generate()
        {
            if (_uuid == null)
            {
                var proxy = (Ottoman.Proxy.IRestProxy)Options["RestProxy"];
                var uuidURIBuilder = new UriBuilder((string)Options["ServerURL"]);
                uuidURIBuilder.Path = "_uuids";
                var response = proxy.Get(uuidURIBuilder.Uri);
                // Here is an example of what is expected to be in the response body: {"uuids":["5d531fd2d85de34f04eaaedce2090cdc"]}
                JObject o = JObject.Parse(response.Body);
                _uuid = o["uuids"][0].ToString().Replace("\"", ""); //The uuid is returned double quoted. The call to Replace strips off the quotes.
            }
            return _uuid + GetNextSequenceNumber();
        }

        private string GetNextSequenceNumber()
        {
            _sequence++;
            return String.Format("{0:X}", _sequence).PadLeft(8,'0');
        }

        public HiLoGenerator()
        {
            Options = new Dictionary<string, object> { {"ServerURL", "http://127.0.0.1:5984"},
                                                       {"RestProxy", new Ottoman.Proxy.RestProxy(new Ottoman.Proxy.HttpClient())} };
        }
    }       
}
