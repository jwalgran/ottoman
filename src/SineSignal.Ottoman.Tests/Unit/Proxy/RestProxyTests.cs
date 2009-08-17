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
			mockHttpClient.Setup(x => x.Request(It.Is<IHttpRequest>(h => h.Url == url && h.Method == HttpMethod.Put))).Returns(mockHttpResponse.Object);

			IRestProxy restProxy = new RestProxy(mockHttpClient.Object);
			IHttpResponse response = restProxy.Put(url);

			mockHttpClient.Verify(x => x.Request(It.Is<IHttpRequest>(h => h.Url == url && h.Method == HttpMethod.Put)));

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
			mockHttpClient.Setup(x => x.Request(It.Is<IHttpRequest>(h => h.Url == url && h.Method == HttpMethod.Delete))).Returns(mockHttpResponse.Object);

			IRestProxy restProxy = new RestProxy(mockHttpClient.Object);
			IHttpResponse response = restProxy.Delete(url);

			mockHttpClient.Verify(x => x.Request(It.Is<IHttpRequest>(h => h.Url == url && h.Method == HttpMethod.Delete)));

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
			mockHttpClient.Setup(x => x.Request(It.Is<IHttpRequest>(h => h.Url == url && h.Method == HttpMethod.Get))).Returns(mockHttpResponse.Object);

			IRestProxy restProxy = new RestProxy(mockHttpClient.Object);
			IHttpResponse response = restProxy.Get(url);

			mockHttpClient.Verify(x => x.Request(It.Is<IHttpRequest>(h => h.Url == url && h.Method == HttpMethod.Get)));

			Assert.IsNotNull(response);
			Assert.AreEqual(mockHttpResponse.Object, response);
		}

		[Test]
		public void Should_create_web_request_with_a_post_method()
		{
			Uri url = new Uri("http://127.0.0.1:5984/test");
			string contentType = "application/json";
			string requestBody = "{\"_id\":\"4B4C3FFB-0F92-4842-8E48-A21D1350A838\",\"name\":\"John Doe\"}";
			string responseBody = "{\"ok\":true,\"id\":\"4B4C3FFB-0F92-4842-8E48-A21D1350A838\",\"rev\":\"1083377286\"}";

			var mockHttpResponse = new Mock<IHttpResponse>();
			mockHttpResponse.Setup(x => x.StatusCode).Returns(HttpStatusCode.Created);
			mockHttpResponse.Setup(x => x.Body).Returns(responseBody);

			var mockHttpClient = new Mock<IHttpClient>();
			mockHttpClient.Setup(x => x.Request(It.Is<IHttpRequest>(h => h.Url == url && h.Method == HttpMethod.Post && h.ContentType == contentType && h.PostData == requestBody))).Returns(mockHttpResponse.Object);

			IRestProxy restProxy = new RestProxy(mockHttpClient.Object);
			IHttpResponse response = restProxy.Post(url, contentType, requestBody);

			mockHttpClient.Verify(x => x.Request(It.Is<IHttpRequest>(h => h.Url == url && h.Method == HttpMethod.Post && h.ContentType == contentType && h.PostData == requestBody)));

			Assert.IsNotNull(response);
			Assert.AreEqual(mockHttpResponse.Object, response);
		}
	}
}