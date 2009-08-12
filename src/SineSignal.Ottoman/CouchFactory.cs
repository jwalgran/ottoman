#region License

// <copyright file="CouchFactory.cs" company="SineSignal, LLC.">
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

using SineSignal.Ottoman.Proxies;
using SineSignal.Ottoman.Serializers;

namespace SineSignal.Ottoman
{
	/// <summary>
	/// Factory pattern for creating CouchInstances.
	/// </summary>
	public static class CouchFactory
	{
		/// <summary>
		/// Creates a CouchInstance.
		/// </summary>
		/// <param name="couchUrl">The url to your CouchDB server.</param>
		/// <returns cref="ICouchInstance">A instatiated CouchInstance.</returns>
		public static ICouchInstance Create(string couchUrl)
		{
			IHttpClient webClient = new HttpClient();
			IRestProxy restProxy = new RestProxy(webClient);
			ISerializer serializer = new JsonSerializer();
			
			return new CouchInstance(couchUrl, restProxy, serializer);
		}
	}
}