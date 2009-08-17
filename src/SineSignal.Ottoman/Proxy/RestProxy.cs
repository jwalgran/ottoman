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
using System.Net;

namespace SineSignal.Ottoman.Proxy
{
	/// <summary>
	/// Used for creating RESTFUL web requests.
	/// </summary>
	public class RestProxy : IRestProxy
	{
		private readonly IHttpClient _httpClient;

		public RestProxy(IHttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		/// <summary>
		/// Creates a PUT request for the specified URL.
		/// </summary>
		/// <param name="url">The URL to make the request to.</param>
		/// <returns>The response from the URL the request was made.</returns>
		public IHttpResponse Put(Uri url)
		{
			return _httpClient.Request(new HttpRequest(url, HttpMethod.Put));
		}

		/// <summary>
		/// Creates a PUT request for the specified URL.
		/// </summary>
		/// <param name="url">The URL to make the request to.</param>
		/// <param name="contentType">The content type of the body.</param>
		/// <param name="body">The body of the request.</param>
		/// <returns>The response from the URL the request was made.</returns>
		public IHttpResponse Put(Uri url, string contentType, string body)
		{
			return _httpClient.Request(new HttpRequest(url, HttpMethod.Put, contentType, body));
		}

		/// <summary>
		/// Creates a DELETE request for the specified URL.
		/// </summary>
		/// <param name="url">The URL to make the request to.</param>
		/// <returns>The response from the URL the request was made.</returns>
		public IHttpResponse Delete(Uri url)
		{
			return _httpClient.Request(new HttpRequest(url, HttpMethod.Delete));
		}

		/// <summary>
		/// Creates a GET request for the specified URL.
		/// </summary>
		/// <param name="url">The URL to make the request to.</param>
		/// <returns>The response from the URL the request was made.</returns>
		public IHttpResponse Get(Uri url)
		{
			return _httpClient.Request(new HttpRequest(url, HttpMethod.Get));
		}

		/// <summary>
		/// Creates a POST request for the specified URL.
		/// </summary>
		/// <param name="url">The URL to make the request to.</param>
		/// <param name="contentType">The content type in the body.</param>
		/// <param name="body">The body of the request.</param>
		/// <returns></returns>
		public IHttpResponse Post(Uri url, string contentType, string body)
		{
			return _httpClient.Request(new HttpRequest(url, HttpMethod.Post, contentType, body));
		}
	}
}