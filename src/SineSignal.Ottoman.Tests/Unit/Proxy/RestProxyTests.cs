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
	public class When_making_a_put_request
	{
		[Test]	
		public void Should_call_httpclient_request_passing_a_new_http_request_with_url_and_a_put_method()
		{
			// Arrange
			Uri requestUrl = new Uri("http://127.0.0.1/test");
			string body = "{\"ok\":true}";
			var mockHttpClient = new Mock<IHttpClient>();
			mockHttpClient.Setup(x => x.Request(It.Is<IHttpRequest>(h => h.Url == requestUrl && h.Method == HttpMethod.Put))).Returns(new HttpResponse(HttpStatusCode.Created, body));
			
			// Act
			IRestProxy restProxy = new RestProxy(mockHttpClient.Object);
			restProxy.Put(requestUrl);
			
			// Assert
			mockHttpClient.Verify(x => x.Request(It.Is<IHttpRequest>(h => h.Url == requestUrl && h.Method == HttpMethod.Put)));
		}
		
		[Test]
		public void Should_return_a_http_response()
		{
			// Arrange
			Uri requestUrl = new Uri("http://127.0.0.1/test");
			string body = "{\"ok\":true}";
			var mockHttpClient = new Mock<IHttpClient>();
			mockHttpClient.Setup(x => x.Request(It.Is<IHttpRequest>(h => h.Url == requestUrl && h.Method == HttpMethod.Put))).Returns(new HttpResponse(HttpStatusCode.Created, body));

			// Act
			IRestProxy restProxy = new RestProxy(mockHttpClient.Object);
			IHttpResponse response = restProxy.Put(requestUrl);
			
			// Assert
			Assert.IsNotNull(response);
			Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
			Assert.AreEqual(body, response.Body);
		}
	}
	
	[TestFixture]
	public class When_making_a_put_request_with_content_type_and_a_body
	{
		[Test]
		public void Should_call_httpclient_request_passing_a_new_http_request_with_url_and_a_put_method_and_content_type_and_body()
		{
			// Arrange
			Uri url = new Uri("http://127.0.0.1:5984/test/4B4C3FFB-0F92-4842-8E48-A21D1350A838");
			string contentType = "application/json";
			string requestBody = "{\"name\":\"John Doe\"}";
			string responseBody = "{\"ok\":true,\"id\":\"4B4C3FFB-0F92-4842-8E48-A21D1350A838\",\"rev\":\"1083377286\"}";

			var mockHttpClient = new Mock<IHttpClient>();
			mockHttpClient.Setup(x => x.Request(It.Is<IHttpRequest>(h => h.Url == url && h.Method == HttpMethod.Put && h.ContentType == contentType && h.PostData == requestBody))).Returns(new HttpResponse(HttpStatusCode.Created, responseBody));

			// Act
			IRestProxy restProxy = new RestProxy(mockHttpClient.Object);
			restProxy.Put(url, contentType, requestBody);

			// Assert
			mockHttpClient.Verify(x => x.Request(It.Is<IHttpRequest>(h => h.Url == url && h.Method == HttpMethod.Put && h.ContentType == contentType && h.PostData == requestBody)));
		}
		
		[Test]
		public void Should_return_a_http_response()
		{
			// Arrange
			Uri url = new Uri("http://127.0.0.1:5984/test/4B4C3FFB-0F92-4842-8E48-A21D1350A838");
			string contentType = "application/json";
			string requestBody = "{\"name\":\"John Doe\"}";
			string responseBody = "{\"ok\":true,\"id\":\"4B4C3FFB-0F92-4842-8E48-A21D1350A838\",\"rev\":\"1083377286\"}";

			var mockHttpClient = new Mock<IHttpClient>();
			mockHttpClient.Setup(x => x.Request(It.Is<IHttpRequest>(h => h.Url == url && h.Method == HttpMethod.Put && h.ContentType == contentType && h.PostData == requestBody))).Returns(new HttpResponse(HttpStatusCode.Created, responseBody));

			// Act
			IRestProxy restProxy = new RestProxy(mockHttpClient.Object);
			IHttpResponse response = restProxy.Put(url, contentType, requestBody);

			// Assert
			Assert.IsNotNull(response);
			Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
			Assert.AreEqual(responseBody, response.Body);
		}
	}

	[TestFixture]
	public class When_making_a_delete_request
	{
		[Test]
		public void Should_call_httpclient_request_passing_a_new_http_request_with_url_and_a_delete_method()
		{
			// Arrange
			Uri requestUrl = new Uri("http://127.0.0.1/test");
			string body = "{\"ok\":true}";
			var mockHttpClient = new Mock<IHttpClient>();
			mockHttpClient.Setup(x => x.Request(It.Is<IHttpRequest>(h => h.Url == requestUrl && h.Method == HttpMethod.Delete))).Returns(new HttpResponse(HttpStatusCode.OK, body));

			// Act
			IRestProxy restProxy = new RestProxy(mockHttpClient.Object);
			restProxy.Delete(requestUrl);

			// Assert
			mockHttpClient.Verify(x => x.Request(It.Is<IHttpRequest>(h => h.Url == requestUrl && h.Method == HttpMethod.Delete)));
		}

		[Test]
		public void Should_return_a_http_response()
		{
			// Arrange
			Uri requestUrl = new Uri("http://127.0.0.1/test");
			string body = "{\"ok\":true}";
			var mockHttpClient = new Mock<IHttpClient>();
			mockHttpClient.Setup(x => x.Request(It.Is<IHttpRequest>(h => h.Url == requestUrl && h.Method == HttpMethod.Delete))).Returns(new HttpResponse(HttpStatusCode.OK, body));

			// Act
			IRestProxy restProxy = new RestProxy(mockHttpClient.Object);
			IHttpResponse response = restProxy.Delete(requestUrl);

			// Assert
			Assert.IsNotNull(response);
			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			Assert.AreEqual(body, response.Body);
		}
	}
	
	[TestFixture]
	public class When_making_a_get_request
	{
		[Test]
		public void Should_call_httpclient_request_passing_a_new_http_request_with_url_and_a_get_method()
		{
			// Arrange
			Uri url = new Uri("http://127.0.0.1:5984/test");
			string body = "{\"ok\":true}";

			var mockHttpClient = new Mock<IHttpClient>();
			mockHttpClient.Setup(x => x.Request(It.Is<IHttpRequest>(h => h.Url == url && h.Method == HttpMethod.Get))).Returns(new HttpResponse(HttpStatusCode.OK, body));
			
			// Act
			IRestProxy restProxy = new RestProxy(mockHttpClient.Object);
			restProxy.Get(url);
			
			// Assert
			mockHttpClient.Verify(x => x.Request(It.Is<IHttpRequest>(h => h.Url == url && h.Method == HttpMethod.Get)));
		}
		
		[Test]
		public void Should_return_a_http_response()
		{
			// Arrange
			Uri url = new Uri("http://127.0.0.1:5984/test");
			string body = "{\"ok\":true}";

			var mockHttpClient = new Mock<IHttpClient>();
			mockHttpClient.Setup(x => x.Request(It.Is<IHttpRequest>(h => h.Url == url && h.Method == HttpMethod.Get))).Returns(new HttpResponse(HttpStatusCode.OK, body));

			// Act
			IRestProxy restProxy = new RestProxy(mockHttpClient.Object);
			IHttpResponse response = restProxy.Get(url);

			// Assert
			mockHttpClient.Verify(x => x.Request(It.Is<IHttpRequest>(h => h.Url == url && h.Method == HttpMethod.Get)));

			Assert.IsNotNull(response);
			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			Assert.AreEqual(body, response.Body);
		}
	}
	
	[TestFixture]
	public class When_making_a_post_request
	{
		[Test]
		public void Should_call_httpclient_request_passing_a_new_http_request_with_url_and_a_post_method_and_content_type_and_body()
		{
			// Arrange
			Uri url = new Uri("http://127.0.0.1:5984/test");
			string contentType = "application/json";
			string requestBody = "{\"name\":\"John Doe\"}";
			string responseBody = "{\"ok\":true,\"id\":\"4B4C3FFB0F9248428E48A21D1350A838\",\"rev\":\"1083377286\"}";

			var mockHttpClient = new Mock<IHttpClient>();
			mockHttpClient.Setup(x => x.Request(It.Is<IHttpRequest>(h => h.Url == url && h.Method == HttpMethod.Post && h.ContentType == contentType && h.PostData == requestBody))).Returns(new HttpResponse(HttpStatusCode.Created, responseBody));
			
			// Act
			IRestProxy restProxy = new RestProxy(mockHttpClient.Object);
			restProxy.Post(url, contentType, requestBody);
			
			// Assert
			mockHttpClient.Verify(x => x.Request(It.Is<IHttpRequest>(h => h.Url == url && h.Method == HttpMethod.Post && h.ContentType == contentType && h.PostData == requestBody)));
		}
		
		[Test]
		public void Should_return_a_http_response()
		{
			// Arrange
			Uri url = new Uri("http://127.0.0.1:5984/test");
			string contentType = "application/json";
			string requestBody = "{\"name\":\"John Doe\"}";
			string responseBody = "{\"ok\":true,\"id\":\"4B4C3FFB0F9248428E48A21D1350A838\",\"rev\":\"1083377286\"}";

			var mockHttpClient = new Mock<IHttpClient>();
			mockHttpClient.Setup(x => x.Request(It.Is<IHttpRequest>(h => h.Url == url && h.Method == HttpMethod.Post && h.ContentType == contentType && h.PostData == requestBody))).Returns(new HttpResponse(HttpStatusCode.Created, responseBody));

			// Act
			IRestProxy restProxy = new RestProxy(mockHttpClient.Object);
			IHttpResponse response = restProxy.Post(url, contentType, requestBody);

			// Assert
			Assert.IsNotNull(response);
			Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
			Assert.AreEqual(responseBody, response.Body);
		}
	}
}