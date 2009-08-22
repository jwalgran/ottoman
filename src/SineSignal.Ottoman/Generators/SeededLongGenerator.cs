#region License

// <copyright file="SeededLongGenerator.cs" company="SineSignal, LLC.">
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
using System.Security.Cryptography;
using System.Text;

using Newtonsoft.Json.Linq;

using SineSignal.Ottoman.Proxy;

namespace SineSignal.Ottoman.Generators
{
    /// <summary>
    /// Used to generate document identifier's seeded with a random value from a CouchDB server then incremented on the client.
    /// </summary>
    class SeededLongGenerator : IGenerator<long>
    {
        private string _uuid;
        private int _sequence;
        private MD5 _md5 = MD5.Create();

        public string ServerURL { get; set; }
        public IRestProxy RestProxy { get; set; }
        public int ReseedInterval { get; set;}

        /// <summary>
        /// Generates a unique document identifier.
        /// </summary>
        /// <returns>A unique integer each time the function is called.</returns>
        public long Generate()
        {
            if (_sequence == ReseedInterval)
            {
                _sequence = 0;
                _uuid = null;
            }

            if (_uuid == null)
            {
                var uuidURIBuilder = new UriBuilder(ServerURL);
                uuidURIBuilder.Path = "_uuids";
                var response = RestProxy.Get(uuidURIBuilder.Uri);
                // Here is an example of what is expected to be in the response body: {"uuids":["5d531fd2d85de34f04eaaedce2090cdc"]}
                JObject o = JObject.Parse(response.Body);
                _uuid = o["uuids"][0].ToString().Replace("\"", ""); //The uuid is returned double quoted. The call to Replace strips off the quotes.
            }
            var stringID = _uuid + GetNextSequenceNumber().ToString("X");
            var hash = _md5.ComputeHash(Encoding.ASCII.GetBytes(stringID));
            return Convert.ToInt64(BitConverter.ToUInt32(hash,0));
        }

        private int GetNextSequenceNumber() { return _sequence++; }

        public SeededLongGenerator(): this("http://127.0.0.1:5984",new RestProxy(new HttpClient()),int.MaxValue){}

        public SeededLongGenerator(string serverURL, IRestProxy restProxy, int reseedInterval)
        {
            ServerURL = serverURL;
            RestProxy = restProxy;
            ReseedInterval = reseedInterval;
        }
    }       
}
