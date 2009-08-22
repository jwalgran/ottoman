#region License

// <copyright file="ServerFactory.cs" company="SineSignal, LLC.">
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

using SineSignal.Ottoman.Proxy;
using SineSignal.Ottoman.Serializers;

namespace SineSignal.Ottoman
{
	/// <summary>
	/// Factory pattern for creating <see cref="Server" /> instances.
	/// </summary>
	public static class ServerFactory
	{
		/// <summary>
		/// Creates a server instance with the default URL of http://127.0.0.1:5984/.
		/// </summary>
		/// <returns>A instantiated server.</returns>
		public static IServer Create()
		{
			return Create("http://127.0.0.1:5984/");
		}

		/// <summary>
		/// Creates a Server instance with the specified URL.
		/// </summary>
		/// <param name="couchUrl">The url to your CouchDB server.</param>
		/// <returns cref="IServer">A instatiated Server.</returns>
		public static IServer Create(string couchUrl)
		{
			IRestClient restClient = new RestClient();
			ISerializer serializer = new JsonSerializer();
			
			return new Server(couchUrl, restClient, serializer);
		}
	}
}