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
	[Category("Unit")]
	public class When_making_a_put_request : OttomanSpecBase<RestClient>
	{
		private Uri Url { get; set; }
		private string Body { get; set; }
		private Mock<IHttpClient> FakeHttpClient { get; set; }
		
		protected override RestClient EstablishContext()
		{
			Url = new Uri("http://127.0.0.1/test");
			Body = "{\"ok\":true}";
			FakeHttpClient = new Mock<IHttpClient>();
			
			return new RestClient(FakeHttpClient.Object);
		}

		[Test]	
		public void Should_call_request_on_HttpClient_passing_a_new_http_request_with_a_url_and_a_put_method()
		{
			FakeHttpClient.Setup(x => x.Request(It.Is<IHttpRequest>(h => h.Url == Url && h.Method == HttpMethod.Put))).Returns(new HttpResponse(HttpStatusCode.Created, Body));
			
			Sut.Put(Url);
			
			FakeHttpClient.Verify(x => x.Request(It.Is<IHttpRequest>(h => h.Url == Url && h.Method == HttpMethod.Put)));
		}
		
		[Test]
		public void Should_return_a_http_response()
		{
			FakeHttpClient.Setup(x => x.Request(It.Is<IHttpRequest>(h => h.Url == Url && h.Method == HttpMethod.Put))).Returns(new HttpResponse(HttpStatusCode.Created, Body));

			IHttpResponse response = Sut.Put(Url);
			
			Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
			Assert.AreEqual(Body, response.Body);
		}
	}
	
	[TestFixture]
	[Category("Unit")]
	public class When_making_a_put_request_with_content_type_and_a_body : OttomanSpecBase<RestClient>
	{
		private Uri Url { get; set; }
		private string ContentType { get; set; }
		private string RequestBody { get; set; }
		private string ResponseBody { get; set; }
		private Mock<IHttpClient> FakeHttpClient { get; set; }

		protected override RestClient EstablishContext()
		{
			Url = new Uri("http://127.0.0.1:5984/test/4B4C3FFB-0F92-4842-8E48-A21D1350A838");
			ContentType = "application/json";
			RequestBody = "{\"name\":\"John Doe\"}";
			ResponseBody = "{\"ok\":true,\"id\":\"4B4C3FFB-0F92-4842-8E48-A21D1350A838\",\"rev\":\"1083377286\"}";
			FakeHttpClient = new Mock<IHttpClient>();

			return new RestClient(FakeHttpClient.Object);
		}
		
		[Test]
		public void Should_call_request_on_HttpClient_passing_a_new_http_request_with_a_url_and_a_put_method_and_a_content_type_and_a_body()
		{
			FakeHttpClient.Setup(x => x.Request(It.Is<IHttpRequest>(h => h.Url == Url && h.Method == HttpMethod.Put && h.ContentType == ContentType && h.PostData == RequestBody))).Returns(new HttpResponse(HttpStatusCode.Created, ResponseBody));

			Sut.Put(Url, ContentType, RequestBody);

			FakeHttpClient.Verify(x => x.Request(It.Is<IHttpRequest>(h => h.Url == Url && h.Method == HttpMethod.Put && h.ContentType == ContentType && h.PostData == RequestBody)));
		}
		
		[Test]
		public void Should_return_a_http_response()
		{
			FakeHttpClient.Setup(x => x.Request(It.Is<IHttpRequest>(h => h.Url == Url && h.Method == HttpMethod.Put && h.ContentType == ContentType && h.PostData == RequestBody))).Returns(new HttpResponse(HttpStatusCode.Created, ResponseBody));

			IHttpResponse response = Sut.Put(Url, ContentType, RequestBody);

			Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
			Assert.AreEqual(ResponseBody, response.Body);
		}
	}

	[TestFixture]
	[Category("Unit")]
	public class When_making_a_delete_request : OttomanSpecBase<RestClient>
	{
		private Uri Url { get; set; }
		private string Body { get; set; }
		private Mock<IHttpClient> FakeHttpClient { get; set; }

		protected override RestClient EstablishContext()
		{
			Url = new Uri("http://127.0.0.1/test");
			Body = "{\"ok\":true}";
			FakeHttpClient = new Mock<IHttpClient>();

			return new RestClient(FakeHttpClient.Object);
		}
		
		[Test]
		public void Should_call_request_on_HttpClient_passing_a_new_http_request_with_a_url_and_a_delete_method()
		{
			FakeHttpClient.Setup(x => x.Request(It.Is<IHttpRequest>(h => h.Url == Url && h.Method == HttpMethod.Delete))).Returns(new HttpResponse(HttpStatusCode.OK, Body));

			Sut.Delete(Url);

			FakeHttpClient.Verify(x => x.Request(It.Is<IHttpRequest>(h => h.Url == Url && h.Method == HttpMethod.Delete)));
		}

		[Test]
		public void Should_return_a_http_response()
		{
			FakeHttpClient.Setup(x => x.Request(It.Is<IHttpRequest>(h => h.Url == Url && h.Method == HttpMethod.Delete))).Returns(new HttpResponse(HttpStatusCode.OK, Body));

			IHttpResponse response = Sut.Delete(Url);

			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			Assert.AreEqual(Body, response.Body);
		}
	}
	
	[TestFixture]
	[Category("Unit")]
	public class When_making_a_get_request : OttomanSpecBase<RestClient>
	{
		private Uri Url { get; set; }
		private string Body { get; set; }
		private Mock<IHttpClient> FakeHttpClient { get; set; }

		protected override RestClient EstablishContext()
		{
			Url = new Uri("http://127.0.0.1/test");
			Body = "{\"ok\":true}";
			FakeHttpClient = new Mock<IHttpClient>();

			return new RestClient(FakeHttpClient.Object);
		}
		
		[Test]
		public void Should_call_request_on_HttpClient_passing_a_new_http_request_with_a_url_and_a_get_method()
		{
			FakeHttpClient.Setup(x => x.Request(It.Is<IHttpRequest>(h => h.Url == Url && h.Method == HttpMethod.Get))).Returns(new HttpResponse(HttpStatusCode.OK, Body));
			
			Sut.Get(Url);
			
			FakeHttpClient.Verify(x => x.Request(It.Is<IHttpRequest>(h => h.Url == Url && h.Method == HttpMethod.Get)));
		}
		
		[Test]
		public void Should_return_a_http_response()
		{
			FakeHttpClient.Setup(x => x.Request(It.Is<IHttpRequest>(h => h.Url == Url && h.Method == HttpMethod.Get))).Returns(new HttpResponse(HttpStatusCode.OK, Body));

			IHttpResponse response = Sut.Get(Url);

			FakeHttpClient.Verify(x => x.Request(It.Is<IHttpRequest>(h => h.Url == Url && h.Method == HttpMethod.Get)));

			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			Assert.AreEqual(Body, response.Body);
		}
	}
	
	[TestFixture]
	[Category("Unit")]
	public class When_making_a_post_request : OttomanSpecBase<RestClient>
	{
		private Uri Url { get; set; }
		private string ContentType { get; set; }
		private string RequestBody { get; set; }
		private string ResponseBody { get; set; }
		private Mock<IHttpClient> FakeHttpClient { get; set; }

		protected override RestClient EstablishContext()
		{
			Url = new Uri("http://127.0.0.1:5984/test");
			ContentType = "application/json";
			RequestBody = "{\"name\":\"John Doe\"}";
			ResponseBody = "{\"ok\":true,\"id\":\"4B4C3FFB-0F92-4842-8E48-A21D1350A838\",\"rev\":\"1083377286\"}";
			FakeHttpClient = new Mock<IHttpClient>();

			return new RestClient(FakeHttpClient.Object);
		}
		
		[Test]
		public void Should_call_request_on_HttpClient_passing_a_new_http_request_with_a_url_and_a_post_method_and_a_content_type_and_a_body()
		{
			FakeHttpClient.Setup(x => x.Request(It.Is<IHttpRequest>(h => h.Url == Url && h.Method == HttpMethod.Post && h.ContentType == ContentType && h.PostData == RequestBody))).Returns(new HttpResponse(HttpStatusCode.Created, ResponseBody));
			
			Sut.Post(Url, ContentType, RequestBody);
			
			FakeHttpClient.Verify(x => x.Request(It.Is<IHttpRequest>(h => h.Url == Url && h.Method == HttpMethod.Post && h.ContentType == ContentType && h.PostData == RequestBody)));
		}
		
		[Test]
		public void Should_return_a_http_response()
		{
			FakeHttpClient.Setup(x => x.Request(It.Is<IHttpRequest>(h => h.Url == Url && h.Method == HttpMethod.Post && h.ContentType == ContentType && h.PostData == RequestBody))).Returns(new HttpResponse(HttpStatusCode.Created, ResponseBody));

			IHttpResponse response = Sut.Post(Url, ContentType, RequestBody);

			Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
			Assert.AreEqual(ResponseBody, response.Body);
		}
	}
}