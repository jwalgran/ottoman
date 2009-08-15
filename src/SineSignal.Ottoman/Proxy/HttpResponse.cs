#region License

// <copyright file="HttpResponse.cs" company="SineSignal, LLC.">
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

using System.Net;

namespace SineSignal.Ottoman.Proxy
{
	/// <summary>
	/// Models <see cref="HttpWebResponse" /> to allow testability within the code.  We can 
	/// easily pass this around after a request has been made and a response has been received.
	/// </summary>
	public class HttpResponse : IHttpResponse
	{
		/// <summary>
		/// Gets or sets the <see cref="HttpStatusCode" /> for the response.
		/// </summary>
		/// <value>The status code.</value>
		public HttpStatusCode StatusCode { get; private set; }

		/// <summary>
		/// Gets or sets the body for the response.
		/// </summary>
		/// <value>The body.</value>
		public string Body { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="HttpResponse"/> class.
		/// </summary>
		/// <param name="statusCode">The status code.</param>
		/// <param name="body">The body.</param>
		public HttpResponse(HttpStatusCode statusCode, string body)
		{
			StatusCode = statusCode;
			Body = body;
		}
	}
}