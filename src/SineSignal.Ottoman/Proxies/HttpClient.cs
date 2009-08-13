#region License

// <copyright file="HttpClient.cs" company="SineSignal, LLC.">
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
using System.IO;
using System.Net;
using System.Text;

namespace SineSignal.Ottoman.Proxies
{
	/// <summary>
	/// A wrapper class for making <see cref="HttpWebRequest" />'s.
	/// </summary>
	public class HttpClient : IHttpClient
	{
		/// <summary>
		/// Makes a HttpWebRequest with the specified IHttpRequest.
		/// </summary>
		/// <param name="httpRequest">The IHttpRequest to use to create the HttpWebRequest.</param>
		/// <returns><see cref="IHttpResponse" /></returns>
		public IHttpResponse Request(IHttpRequest httpRequest)
		{
			HttpWebRequest httpWebRequest = WebRequest.Create(httpRequest.Url) as HttpWebRequest;
			httpWebRequest.Method = httpRequest.Method;
			httpWebRequest.Timeout = -1;

			if (!String.IsNullOrEmpty(httpRequest.ContentType))
			{
				httpWebRequest.ContentType = httpRequest.ContentType;
			}

			if (!String.IsNullOrEmpty(httpRequest.PostData))
			{
				byte[] bytes = UTF8Encoding.UTF8.GetBytes(httpRequest.PostData);
				httpWebRequest.ContentLength = bytes.Length;
				using (Stream requestStream = httpWebRequest.GetRequestStream())
				{
					requestStream.Write(bytes, 0, bytes.Length);
				}
			}

			HttpWebResponse httpWebResponse;
			string response = String.Empty;
			
			try
			{
				httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse;
			}
			catch (WebException e)
			{
				httpWebResponse = e.Response as HttpWebResponse;
			}
			
			using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
			{
				response = streamReader.ReadToEnd();
			}
			
			return new HttpResponse(httpWebResponse.StatusCode, response);
		}
	}
}