#region License

// <copyright file="HttpRequest.cs" company="SineSignal, LLC.">
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
	/// Models information used for creating web requests.
	/// </summary>
	public class HttpRequest : IHttpRequest
	{
		/// <summary>
		/// Gets or sets the URL used.
		/// </summary>
		/// <value>The URL.</value>
		public Uri Url { get; private set; }

		/// <summary>
		/// Gets or sets the method used.
		/// </summary>
		/// <value>The method.</value>
		public string Method { get; private set; }

		/// <summary>
		/// Gets or sets the content type used.
		/// </summary>
		/// <value>The content type.</value>
		public string ContentType { get; private set; }

		/// <summary>
		/// Gets or sets the post data used.
		/// </summary>
		/// <value>The post data.</value>
		public string PostData { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="HttpRequest"/> class.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <param name="method">The method.</param>
		public HttpRequest(Uri url, string method)
		{
			Url = url;
			Method = method;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HttpRequest"/> class.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <param name="method">The method.</param>
		/// <param name="contentType">Type of the content.</param>
		/// <param name="postData">The post data.</param>
		public HttpRequest(Uri url, string method, string contentType, string postData)
		{
			Url = url;
			Method = method;
			ContentType = contentType;
			PostData = postData;
		}
	}
}