#region License

// <copyright file="Proxy.cs" company="SineSignal, LLC.">
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

namespace SineSignal.Ottoman
{
	public class Proxy
	{
		private readonly Uri _uri;

		public Uri Uri { get { return _uri; } }

		public Proxy(string url)
		{
			// Validate input
			if (String.IsNullOrEmpty(url))
				throw new ArgumentNullException("url", "The value cannot be null or an empty string.");

			bool isValidUri = Uri.TryCreate(url, UriKind.Absolute, out _uri);
			if (!isValidUri)
				throw new ArgumentException("The value is invalid, please pass a valid Uri.", "url");

			if (!_uri.Scheme.Equals("http") && !_uri.Scheme.Equals("https"))
				throw new ArgumentException("The value is not using the http or https protocol.  This is not allowed since CouchDB uses REST and Http for communication.", "url");
		}
	}
}