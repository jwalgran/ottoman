#region License

// <copyright file="RestProxy.cs" company="SineSignal, LLC.">
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

namespace SineSignal.Ottoman.Proxies
{
	/// <summary>
	/// Used for creating RESTFUL web requests.
	/// </summary>
	public class RestProxy : IRestProxy
	{
		/// <summary>
		/// Creates a PUT request for the specified URL.
		/// </summary>
		/// <param name="url">The URL to make the request to.</param>
		/// <returns>The response from the URL the request was made.</returns>
		public IWebResponse Put(Uri url)
		{
			throw new NotImplementedException();
		}
	}
}