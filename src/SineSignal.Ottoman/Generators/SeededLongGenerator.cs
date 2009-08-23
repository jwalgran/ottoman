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
using System.Security.Cryptography;
using System.Text;

namespace SineSignal.Ottoman.Generators
{
    /// <summary>
    /// Used to generate document identifier's seeded with a random value from a CouchDB server then incremented on the client.
    /// </summary>
    public class SeededLongGenerator : IGenerator<long>
    {
		private Guid _uuid; 
		private int _sequence;
		private MD5 _md5 = MD5.Create();
		
		public IServer Server { get; private set; }
		public int ReseedInterval { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="SeededLongGenerator"/> class.
		/// </summary>
		/// <param name="server">The server to use for requesting UUID's from CouchDB.</param>
		public SeededLongGenerator(IServer server) : this(server, Int32.MaxValue)
		{
		}

    	/// <summary>
		/// Initializes a new instance of the <see cref="SeededLongGenerator"/> class.
		/// </summary>
		/// <param name="server">The server to use for requesting UUID's from CouchDB.</param>
		/// <param name="reseedInterval">The interval to use when reseeding the sequence.</param>
		public SeededLongGenerator(IServer server, int reseedInterval)
		{
			Server = server;
			ReseedInterval = reseedInterval;
		}

        /// <summary>
        /// Generates a unique document identifier.
        /// </summary>          
        /// <returns>A unique integer each time the function is called.</returns>
        public long Generate()
        {
            if (_sequence == ReseedInterval)
            {
                _sequence = 0;
                _uuid = Guid.Empty;
            }

            if (_uuid == Guid.Empty)
            {
            	_uuid = Server.GetUuids(1)[0];
            }
            
            var stringID = _uuid + GetNextSequenceNumber().ToString("X");
            var hash = _md5.ComputeHash(Encoding.ASCII.GetBytes(stringID));
            
            return Convert.ToInt64(BitConverter.ToUInt32(hash,0));
        }

        private int GetNextSequenceNumber() { return _sequence++; }

    }       
}
