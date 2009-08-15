#region License

// <copyright file="ServerTests.cs" company="SineSignal, LLC.">
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
using SineSignal.Ottoman.Serializers;

namespace SineSignal.Ottoman.Tests.Unit
{
	// TODO:  Now that we have a pattern emerging, clean these tests up
	[TestFixture]
	public class ServerTests
	{
		[Test]
		[Row(null)]
		[Row("")]
		[ExpectedArgumentNullException]
		public void Should_throw_argument_null_exception_when_url_is_null_or_empty_string(string baseUrl)
		{
			var mockRestProxy = new Mock<IRestProxy>();
			var mockSerializer = new Mock<ISerializer>();
			
			new Server(baseUrl, mockRestProxy.Object, mockSerializer.Object);
		}

		[Test]
		[ExpectedArgumentException]
		public void Should_throw_argument_exception_when_url_is_a_random_string()
		{
			var mockRestProxy = new Mock<IRestProxy>();
			var mockSerializer = new Mock<ISerializer>();
			
			new Server("some string value", mockRestProxy.Object, mockSerializer.Object);
		}

		[Test]
		[ExpectedArgumentException]
		public void Should_throw_argument_exception_when_url_is_relative()
		{
			var mockRestProxy = new Mock<IRestProxy>();
			var mockSerializer = new Mock<ISerializer>();
			
			new Server("../somepath", mockRestProxy.Object, mockSerializer.Object);
		}

		[Test]
		[Row("ftp://127.0.0.1/somepath")]
		[Row("file:///C:/somepath")]
		[ExpectedArgumentException]
		public void Should_throw_argument_exception_when_url_is_not_using_http_or_https_scheme(string baseUrl)
		{
			var mockRestProxy = new Mock<IRestProxy>();
			var mockSerializer = new Mock<ISerializer>();
			
			IServer couchServer = new Server(baseUrl, mockRestProxy.Object, mockSerializer.Object);

			Assert.IsNotNull(couchServer.Url);
			Assert.AreEqual(baseUrl, couchServer.Url.ToString());
		}

		[Test]
		[Row("http://127.0.0.1:5984/","http")]
		[Row("https://domain.com:5984/","https")]
		public void Should_set_uri_for_couch_location_when_given_a_valid_uri(string baseUrl, string expectedScheme)
		{
			var mockRestProxy = new Mock<IRestProxy>();
			var mockSerializer = new Mock<ISerializer>();

			IServer couchServer = new Server(baseUrl, mockRestProxy.Object, mockSerializer.Object);

			Assert.AreEqual(baseUrl, couchServer.Url.ToString());
			Assert.AreEqual(expectedScheme, couchServer.Url.Scheme);
		}

		[Test]
		[ExpectedArgumentNullException]
		public void Should_throw_argument_null_exception_when_proxy_is_null()
		{
			string url = GetValidUrl();
			var mockSerializer = new Mock<ISerializer>();
			
			new Server(url, null, mockSerializer.Object);
		}

		[Test]
		public void Should_set_Proxy_when_given_an_instantiated_proxy()
		{
			string url = GetValidUrl();
			var mockProxy = new Mock<IRestProxy>();
			var mockSerializer = new Mock<ISerializer>();

			IServer couchServer = new Server(url, mockProxy.Object, mockSerializer.Object);

			Assert.IsNotNull(couchServer.RestProxy);
			Assert.AreEqual(mockProxy.Object, couchServer.RestProxy);
		}

		[Test]
		[ExpectedArgumentNullException]
		public void Should_throw_argument_null_exception_when_serializer_is_null()
		{
			string url = GetValidUrl();
			var mockProxy = new Mock<IRestProxy>();
			
			new Server(url, mockProxy.Object, null);
		}

		[Test]
		public void Should_set_Serializer_when_given_an_instantiated_serializer()
		{
			string url = GetValidUrl();
			var mockProxy = new Mock<IRestProxy>();
			var mockSerializer = new Mock<ISerializer>();

			IServer couchServer = new Server(url, mockProxy.Object, mockSerializer.Object);

			Assert.IsNotNull(couchServer.Serializer);
			Assert.AreEqual(mockSerializer.Object, couchServer.Serializer);
		}

		[Test]
		[ExpectedArgumentNullException]
		public void Should_throw_an_argument_null_exception_when_CreateDatabase_is_called_with_a_null_string()
		{
			string url = GetValidUrl();
			var mockRestProxy = new Mock<IRestProxy>();
			var mockSerializer = new Mock<ISerializer>();
			
			IServer couchServer = new Server(url, mockRestProxy.Object, mockSerializer.Object);
			couchServer.CreateDatabase(null);
		}

		[Test]
		[Row(null)]
		[Row("")]
		[ExpectedArgumentNullException]
		public void Should_throw_an_argument_null_exception_when_CreateDatabase_is_called_with_a_null_or_empty_string(string name)
		{
			string url = GetValidUrl();
			var mockRestProxy = new Mock<IRestProxy>();
			var mockSerializer = new Mock<ISerializer>();

			IServer couchServer = new Server(url, mockRestProxy.Object, mockSerializer.Object);
			couchServer.CreateDatabase(name);
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

			IServer couchServer = new Server(url, mockRestProxy.Object, mockSerializer.Object);
			couchServer.CreateDatabase(databaseName);

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
				IServer couchServer = new Server(url, mockRestProxy.Object, mockSerializer.Object);
				couchServer.CreateDatabase(databaseName);
			}
			catch (CannotCreateDatabaseException e)
			{
				cannotCreateDatabaseException = e;
			}

			Assert.AreEqual(String.Format("Failed to create database '{0}'", databaseName), cannotCreateDatabaseException.Message);
			Assert.AreEqual("file_exists", cannotCreateDatabaseException.CouchError.Error);
			Assert.AreEqual("The database could not be created, the file already exists.", cannotCreateDatabaseException.CouchError.Reason);
			Assert.AreEqual(mockHttpResponse.Object, cannotCreateDatabaseException.RawResponse);

			mockRestProxy.Verify(x => x.Put(requestUrl.Uri), Times.AtLeastOnce());
			mockSerializer.Verify(x => x.Deserialize<CouchError>(body), Times.AtLeastOnce());
		}

		[Test]
		[Row(null)]
		[Row("")]
		[ExpectedArgumentNullException]
		public void Should_throw_an_argument_null_exception_when_DeleteDatabase_is_called_with_a_null_or_empty_string(string name)
		{
			string url = GetValidUrl();
			var mockRestProxy = new Mock<IRestProxy>();
			var mockSerializer = new Mock<ISerializer>();

			IServer couchServer = new Server(url, mockRestProxy.Object, mockSerializer.Object);
			couchServer.DeleteDatabase(name);
		}
		
		[Test]
		[Row("test")]
		public void Should_be_able_to_delete_database_when_given_a_valid_name(string databaseName)
		{
			string url = GetValidUrl();
			UriBuilder requestUrl = new UriBuilder(new Uri(url));
			requestUrl.Path = databaseName;
			string body = "{\"ok\":true}";

			var mockHttpResponse = new Mock<IHttpResponse>();
			mockHttpResponse.Setup(x => x.StatusCode).Returns(HttpStatusCode.OK);
			mockHttpResponse.Setup(x => x.Body).Returns(body);
			
			var mockRestProxy = new Mock<IRestProxy>();
			mockRestProxy.Setup(x => x.Delete(requestUrl.Uri)).Returns(mockHttpResponse.Object);

			var mockSerializer = new Mock<ISerializer>();

			IServer couchServer = new Server(url, mockRestProxy.Object, mockSerializer.Object);
			couchServer.DeleteDatabase(databaseName);

			mockRestProxy.Verify(x => x.Delete(requestUrl.Uri), Times.AtLeastOnce());
		}

		[Test]
		public void Should_throw_cannot_delete_database_exception_when_an_error_is_given_in_the_response()
		{
			string url = GetValidUrl();
			string databaseName = "test";
			UriBuilder requestUrl = new UriBuilder(new Uri(url));
			requestUrl.Path = databaseName;
			string body = "{\"error\":\"not_found\",\"reason\":\"missing\"}";

			var mockHttpResponse = new Mock<IHttpResponse>();
			mockHttpResponse.Setup(x => x.StatusCode).Returns(HttpStatusCode.NotFound);
			mockHttpResponse.Setup(x => x.Body).Returns(body);

			var mockRestProxy = new Mock<IRestProxy>();
			mockRestProxy.Setup(x => x.Delete(requestUrl.Uri)).Returns(mockHttpResponse.Object);

			var mockSerializer = new Mock<ISerializer>();
			mockSerializer.Setup(x => x.Deserialize<CouchError>(body)).Returns(new CouchError("not_found", "missing"));

			CannotDeleteDatabaseException cannotDeleteDatabaseException = null;
			try
			{
				IServer couchServer = new Server(url, mockRestProxy.Object, mockSerializer.Object);
				couchServer.DeleteDatabase(databaseName);
			}
			catch (CannotDeleteDatabaseException e)
			{
				cannotDeleteDatabaseException = e;
			}

			Assert.AreEqual(String.Format("Failed to delete database '{0}'", databaseName), cannotDeleteDatabaseException.Message);
			Assert.AreEqual("not_found", cannotDeleteDatabaseException.CouchError.Error);
			Assert.AreEqual("missing", cannotDeleteDatabaseException.CouchError.Reason);
			Assert.AreEqual(mockHttpResponse.Object, cannotDeleteDatabaseException.RawResponse);

			mockRestProxy.Verify(x => x.Delete(requestUrl.Uri), Times.AtLeastOnce());
			mockSerializer.Verify(x => x.Deserialize<CouchError>(body), Times.AtLeastOnce());
		}

		[Test]
		[Row(null)]
		[Row("")]
		[ExpectedArgumentNullException]
		public void Should_throw_an_argument_null_exception_when_GetDatabase_is_called_with_a_null_or_empty_string(string name)
		{
			string url = GetValidUrl();
			var mockRestProxy = new Mock<IRestProxy>();
			var mockSerializer = new Mock<ISerializer>();

			IServer couchServer = new Server(url, mockRestProxy.Object, mockSerializer.Object);
			IDatabase database = couchServer.GetDatabase(name);
		}
		
		[Test]
		[Row("test")]
		public void Should_be_able_to_retrieve_a_couch_database_when_given_a_valid_name(string name)
		{
			string url = GetValidUrl();
			UriBuilder requestUrl = new UriBuilder(new Uri(url));
			requestUrl.Path = name;
			string body = "{\"db_name\":\"test\",\"doc_count\":0,\"doc_del_count\":0,\"update_seq\":0,\"purge_seq\":0,\"compact_running\":false,\"disk_size\":79,\"instance_start_time\":\"1250175373642458\",\"disk_format_version\":4}";
			
			var mockHttpResponse = new Mock<IHttpResponse>();
			mockHttpResponse.Setup(x => x.StatusCode).Returns(HttpStatusCode.OK);
			mockHttpResponse.Setup(x => x.Body).Returns(body);
			
			var mockRestProxy = new Mock<IRestProxy>();
			mockRestProxy.Setup(x => x.Get(requestUrl.Uri)).Returns(mockHttpResponse.Object);
			
			var mockSerializer = new Mock<ISerializer>();
			mockSerializer.Setup(x => x.Deserialize<Database>(body)).Returns(new Database(name, 0, 0, 0, 0, false, 79, "1250175373642458", 4));

			IServer couchServer = new Server(url, mockRestProxy.Object, mockSerializer.Object);
			IDatabase database = couchServer.GetDatabase(name);

			mockRestProxy.Verify(x => x.Get(requestUrl.Uri), Times.AtLeastOnce());
			mockSerializer.Verify(x => x.Deserialize<Database>(body));

			Assert.IsNotNull(database);
			Assert.AreEqual(name, database.Name);
			Assert.AreEqual(0, database.DocCount);
			Assert.AreEqual(0, database.DocDelCount);
			Assert.AreEqual(0, database.UpdateSequence);
			Assert.AreEqual(0, database.PurgeSequence);
			Assert.AreEqual(false, database.CompactRunning);
			Assert.AreEqual(79, database.DiskSize);
			Assert.AreEqual("1250175373642458", database.InstanceStartTime);
			Assert.AreEqual(4, database.DiskFormatVersion);
		}
		
		[Test]
		public void Should_throw_cannot_get_database_exception_when_an_error_is_given_in_the_response()
		{
			string url = GetValidUrl();
			string databaseName = "test";
			UriBuilder requestUrl = new UriBuilder(new Uri(url));
			requestUrl.Path = databaseName;
			string body = "{\"error\":\"not_found\",\"reason\":\"no_db_file\"}";

			var mockHttpResponse = new Mock<IHttpResponse>();
			mockHttpResponse.Setup(x => x.StatusCode).Returns(HttpStatusCode.NotFound);
			mockHttpResponse.Setup(x => x.Body).Returns(body);

			var mockRestProxy = new Mock<IRestProxy>();
			mockRestProxy.Setup(x => x.Get(requestUrl.Uri)).Returns(mockHttpResponse.Object);

			var mockSerializer = new Mock<ISerializer>();
			mockSerializer.Setup(x => x.Deserialize<CouchError>(body)).Returns(new CouchError("not_found", "no_db_file"));

			CannotGetDatabaseException cannotGetDatabaseException = null;
			try
			{
				IServer couchServer = new Server(url, mockRestProxy.Object, mockSerializer.Object);
				couchServer.GetDatabase(databaseName);
			}
			catch (CannotGetDatabaseException e)
			{
				cannotGetDatabaseException = e;
			}

			Assert.AreEqual(String.Format("Failed to get database '{0}'", databaseName), cannotGetDatabaseException.Message);
			Assert.AreEqual("not_found", cannotGetDatabaseException.CouchError.Error);
			Assert.AreEqual("no_db_file", cannotGetDatabaseException.CouchError.Reason);
			Assert.AreEqual(mockHttpResponse.Object, cannotGetDatabaseException.RawResponse);

			mockRestProxy.Verify(x => x.Get(requestUrl.Uri), Times.AtLeastOnce());
			mockSerializer.Verify(x => x.Deserialize<CouchError>(body), Times.AtLeastOnce());
		}
		
		[Test]
		public void Should_be_able_to_retrieve_a_list_of_databases()
		{
			string url = GetValidUrl();
			UriBuilder requestUrl = new UriBuilder(new Uri(url));
			requestUrl.Path = "_all_dbs";
			string body = "[\"test1\",\"test2\"]";

			var mockHttpResponse = new Mock<IHttpResponse>();
			mockHttpResponse.Setup(x => x.StatusCode).Returns(HttpStatusCode.OK);
			mockHttpResponse.Setup(x => x.Body).Returns(body);
			
			var mockRestProxy = new Mock<IRestProxy>();
			mockRestProxy.Setup(x => x.Get(requestUrl.Uri)).Returns(mockHttpResponse.Object);
			
			var mockSerializer = new Mock<ISerializer>();
			mockSerializer.Setup(x => x.Deserialize<string[]>(body)).Returns(new string[] {"test1", "test2"});

			IServer couchServer = new Server(url, mockRestProxy.Object, mockSerializer.Object);
			string[] databases = couchServer.GetDatabases();

			mockRestProxy.Verify(x => x.Get(requestUrl.Uri));
			mockSerializer.Verify(x => x.Deserialize<string[]>(body));

			Assert.IsNotNull(databases);
			Assert.AreEqual(2, databases.Length);
		}

		private string GetValidUrl()
		{
			return "http://127.0.0.1:5984/";
		}
	}
}