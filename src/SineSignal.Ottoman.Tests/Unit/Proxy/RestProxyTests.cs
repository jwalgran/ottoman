#region License

// <copyright file="RestProxyTests.cs" company="SineSignal, LLC.">
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

using MbUnit.Framework;
using Moq;

using SineSignal.Ottoman.Proxy;

namespace SineSignal.Ottoman.Tests.Unit.Proxy
{
	[TestFixture]
	public class RestProxyTests
	{
		[Test]
		public void Should_create_web_request_with_a_put_method()
		{
			Uri url = new Uri("http://127.0.0.1:5984/test");
			string body = "{\"ok\":true}";

			var mockHttpResponse = new Mock<IHttpResponse>();
			mockHttpResponse.Setup(x => x.StatusCode).Returns(HttpStatusCode.Created);
			mockHttpResponse.Setup(x => x.Body).Returns(body);

			var mockHttpClient = new Mock<IHttpClient>();
			mockHttpClient.Setup(x => x.Request(It.Is<IHttpRequest>(h => h.Url == url && h.Method == WebRequestMethods.Http.Put))).Returns(mockHttpResponse.Object);

			IRestProxy restProxy = new RestProxy(mockHttpClient.Object);
			IHttpResponse response = restProxy.Put(url);

			mockHttpClient.Verify(x => x.Request(It.Is<IHttpRequest>(h => h.Url == url && h.Method == WebRequestMethods.Http.Put)));

			Assert.IsNotNull(response);
			Assert.AreEqual(mockHttpResponse.Object, response);
		}

		[Test]
		public void Should_create_web_request_with_a_delete_method()
		{
			Uri url = new Uri("http://127.0.0.1:5984/test");
			string body = "{\"ok\":true}";

			var mockHttpResponse = new Mock<IHttpResponse>();
			mockHttpResponse.Setup(x => x.StatusCode).Returns(HttpStatusCode.OK);
			mockHttpResponse.Setup(x => x.Body).Returns(body);

			var mockHttpClient = new Mock<IHttpClient>();
			mockHttpClient.Setup(x => x.Request(It.Is<IHttpRequest>(h => h.Url == url && h.Method == "DELETE"))).Returns(mockHttpResponse.Object);

			IRestProxy restProxy = new RestProxy(mockHttpClient.Object);
			IHttpResponse response = restProxy.Delete(url);

			mockHttpClient.Verify(x => x.Request(It.Is<IHttpRequest>(h => h.Url == url && h.Method == "DELETE")));

			Assert.IsNotNull(response);
			Assert.AreEqual(mockHttpResponse.Object, response);
		}

		[Test]
		public void Should_create_web_request_with_a_get_method()
		{
			Uri url = new Uri("http://127.0.0.1:5984/test");
			string body = "{\"ok\":true}";

			var mockHttpResponse = new Mock<IHttpResponse>();
			mockHttpResponse.Setup(x => x.StatusCode).Returns(HttpStatusCode.OK);
			mockHttpResponse.Setup(x => x.Body).Returns(body);

			var mockHttpClient = new Mock<IHttpClient>();
			mockHttpClient.Setup(x => x.Request(It.Is<IHttpRequest>(h => h.Url == url && h.Method == WebRequestMethods.Http.Get))).Returns(mockHttpResponse.Object);

			IRestProxy restProxy = new RestProxy(mockHttpClient.Object);
			IHttpResponse response = restProxy.Get(url);

			mockHttpClient.Verify(x => x.Request(It.Is<IHttpRequest>(h => h.Url == url && h.Method == WebRequestMethods.Http.Get)));

			Assert.IsNotNull(response);
			Assert.AreEqual(mockHttpResponse.Object, response);
		}
	}
}