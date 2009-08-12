#region License

// <copyright file="CouchInstanceTests.cs" company="SineSignal, LLC.">
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

using SineSignal.Ottoman.Proxies;
using SineSignal.Ottoman.Serializers;

namespace SineSignal.Ottoman.Tests.Unit
{
	// TODO:  Now that we have a pattern emerging, clean these tests up
	[TestFixture]
	public class CouchInstanceTests
	{
		[Test]
		[ExpectedArgumentNullException]
		public void Should_throw_argument_null_exception_when_url_is_null_or_empty_string()
		{
			var mockRestProxy = new Mock<IRestProxy>();
			var mockSerializer = new Mock<ISerializer>();
			
			new CouchInstance(null, mockRestProxy.Object, mockSerializer.Object);
		}

		[Test]
		[ExpectedArgumentException]
		public void Should_throw_argument_exception_when_url_is_a_random_string()
		{
			var mockRestProxy = new Mock<IRestProxy>();
			var mockSerializer = new Mock<ISerializer>();
			
			new CouchInstance("some string value", mockRestProxy.Object, mockSerializer.Object);
		}

		[Test]
		[ExpectedArgumentException]
		public void Should_throw_argument_exception_when_url_is_relative()
		{
			var mockRestProxy = new Mock<IRestProxy>();
			var mockSerializer = new Mock<ISerializer>();
			
			new CouchInstance("../somepath", mockRestProxy.Object, mockSerializer.Object);
		}

		[Test]
		[ExpectedArgumentException]
		public void Should_throw_argument_exception_when_url_is_not_using_http_or_https_scheme()
		{
			string url = "ftp://127.0.0.1/somepath";
			var mockRestProxy = new Mock<IRestProxy>();
			var mockSerializer = new Mock<ISerializer>();
			
			ICouchInstance couchInstance = new CouchInstance(url, mockRestProxy.Object, mockSerializer.Object);

			Assert.IsNotNull(couchInstance.Url);
			Assert.AreEqual(url, couchInstance.Url.ToString());
		}

		[Test]
		public void Should_set_uri_for_couch_location_when_given_a_valid_http_uri()
		{
			string url = "http://127.0.0.1:5984/";
			var mockRestProxy = new Mock<IRestProxy>();
			var mockSerializer = new Mock<ISerializer>();

			ICouchInstance couchInstance = new CouchInstance(url, mockRestProxy.Object, mockSerializer.Object);

			Assert.AreEqual(url, couchInstance.Url.ToString());
			Assert.AreEqual("http", couchInstance.Url.Scheme);
		}

		[Test]
		public void Should_set_uri_for_couch_location_when_given_a_valid_https_uri()
		{
			string url = "https://domain.com:5984/";
			var mockRestProxy = new Mock<IRestProxy>();
			var mockSerializer = new Mock<ISerializer>();

			ICouchInstance couchInstance = new CouchInstance(url, mockRestProxy.Object, mockSerializer.Object);

			Assert.AreEqual(url, couchInstance.Url.ToString());
			Assert.AreEqual("https", couchInstance.Url.Scheme);
		}

		[Test]
		[ExpectedArgumentNullException]
		public void Should_throw_argument_null_exception_when_proxy_is_null()
		{
			string url = GetValidUrl();
			var mockSerializer = new Mock<ISerializer>();
			
			new CouchInstance(url, null, mockSerializer.Object);
		}

		[Test]
		public void Should_set_Proxy_when_given_an_instantiated_proxy()
		{
			string url = GetValidUrl();
			var mockProxy = new Mock<IRestProxy>();
			var mockSerializer = new Mock<ISerializer>();

			ICouchInstance couchInstance = new CouchInstance(url, mockProxy.Object, mockSerializer.Object);

			Assert.IsNotNull(couchInstance.RestProxy);
			Assert.AreEqual(mockProxy.Object, couchInstance.RestProxy);
		}

		[Test]
		[ExpectedArgumentNullException]
		public void Should_throw_argument_null_exception_when_serializer_is_null()
		{
			string url = GetValidUrl();
			var mockProxy = new Mock<IRestProxy>();
			
			new CouchInstance(url, mockProxy.Object, null);
		}

		[Test]
		public void Should_set_Serializer_when_given_an_instantiated_serializer()
		{
			string url = GetValidUrl();
			var mockProxy = new Mock<IRestProxy>();
			var mockSerializer = new Mock<ISerializer>();

			ICouchInstance couchInstance = new CouchInstance(url, mockProxy.Object, mockSerializer.Object);

			Assert.IsNotNull(couchInstance.Serializer);
			Assert.AreEqual(mockSerializer.Object, couchInstance.Serializer);
		}

		[Test]
		[ExpectedArgumentNullException]
		public void Should_throw_an_argument_null_exception_when_CreateDatabase_is_called_with_a_null_string()
		{
			string url = GetValidUrl();
			var mockRestProxy = new Mock<IRestProxy>();
			var mockSerializer = new Mock<ISerializer>();
			
			ICouchInstance couchInstance = new CouchInstance(url, mockRestProxy.Object, mockSerializer.Object);
			couchInstance.CreateDatabase(null);
		}

		[Test]
		[ExpectedArgumentNullException]
		public void Should_throw_an_argument_null_exception_when_CreateDatabase_is_called_with_an_empty_string()
		{
			string url = GetValidUrl();
			var mockRestProxy = new Mock<IRestProxy>();
			var mockSerializer = new Mock<ISerializer>();
			
			ICouchInstance couchInstance = new CouchInstance(url, mockRestProxy.Object, mockSerializer.Object);
			couchInstance.CreateDatabase("");
		}
		
		[Test]
		public void Should_create_database_when_given_a_valid_name()
		{
			string url = GetValidUrl();
			string databaseName = "test";
			UriBuilder requestUrl = new UriBuilder(new Uri(url));
			requestUrl.Path = databaseName;
			string body = "{\"ok\":true}";

			var mockHttpResponse = new Mock<IHttpResponse>();
			mockHttpResponse.Setup(x => x.StatusCode).Returns(HttpStatusCode.Created);
			mockHttpResponse.Setup(x => x.Body).Returns(body);
			
			var mockRestProxy = new Mock<IRestProxy>();
			mockRestProxy.Setup(x => x.Put(requestUrl.Uri)).Returns(mockHttpResponse.Object);

			var mockSerializer = new Mock<ISerializer>();
			
			ICouchInstance couchInstance = new CouchInstance(url, mockRestProxy.Object, mockSerializer.Object);
			couchInstance.CreateDatabase(databaseName);

			mockRestProxy.Verify(x => x.Put(requestUrl.Uri), Times.AtLeastOnce());
		}
		
		[Test]
		public void Should_throw_cannot_create_database_exception_when_an_error_is_given_in_the_response()
		{
			string url = GetValidUrl();
			string databaseName = "test";
			UriBuilder requestUrl = new UriBuilder(new Uri(url));
			requestUrl.Path = databaseName;
			string body = "{\"error\":\"file_exists\",\"reason\":\"The database could not be created, the file already exists.\"}";

			var mockHttpResponse = new Mock<IHttpResponse>();
			mockHttpResponse.Setup(x => x.StatusCode).Returns(HttpStatusCode.PreconditionFailed);
			mockHttpResponse.Setup(x => x.Body).Returns(body);

			var mockRestProxy = new Mock<IRestProxy>();
			mockRestProxy.Setup(x => x.Put(requestUrl.Uri)).Returns(mockHttpResponse.Object);

			var mockSerializer = new Mock<ISerializer>();
			mockSerializer.Setup(x => x.Deserialize<CouchError>(body)).Returns(new CouchError("file_exists", "The database could not be created, the file already exists."));

			CannotCreateDatabaseException cannotCreateDatabaseException = null;
			try
			{
				ICouchInstance couchInstance = new CouchInstance(url, mockRestProxy.Object, mockSerializer.Object);
				couchInstance.CreateDatabase(databaseName);
			}
			catch (CannotCreateDatabaseException e)
			{
				cannotCreateDatabaseException = e;
			}

			Assert.AreEqual(String.Format("Failed to create database '{0}'", databaseName), cannotCreateDatabaseException.Message);
			Assert.AreEqual("file_exists", cannotCreateDatabaseException.Error.Error);
			Assert.AreEqual("The database could not be created, the file already exists.", cannotCreateDatabaseException.Error.Reason);
			Assert.AreEqual(mockHttpResponse.Object, cannotCreateDatabaseException.RawResponse);

			mockRestProxy.Verify(x => x.Put(requestUrl.Uri), Times.AtLeastOnce());
			mockSerializer.Verify(x => x.Deserialize<CouchError>(body), Times.AtLeastOnce());
		}

		private string GetValidUrl()
		{
			return "http://127.0.0.1:5984/";
		}
	}
}