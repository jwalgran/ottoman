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
	[TestFixture] 
	public class When_instantiating_a_new_server_instance
	{
		[Test]
		[Row(null)]
		[Row("")]
		[ExpectedArgumentNullException]
		public void Should_throw_argument_null_exception_when_url_is_null_or_empty_string(string baseUrl)
		{
			// Arrange
			var mockRestProxy = new Mock<IRestProxy>();
			var mockSerializer = new Mock<ISerializer>();
			
			// Act
			new Server(baseUrl, mockRestProxy.Object, mockSerializer.Object);
		}

		[Test]
		[ExpectedException(typeof(UriFormatException), "The value is invalid, please pass a valid Uri.")]
		public void Should_throw_uri_format_exception_when_url_is_a_random_string()
		{
			// Arrange
			var mockRestProxy = new Mock<IRestProxy>();
			var mockSerializer = new Mock<ISerializer>();
			
			// Act
			new Server("some string value", mockRestProxy.Object, mockSerializer.Object);
		}

		[Test]
		[ExpectedException(typeof(UriFormatException), "The value is invalid, please pass a valid Uri.")]
		public void Should_throw_uri_format_exception_when_url_is_relative()
		{
			// Arrange
			var mockRestProxy = new Mock<IRestProxy>();
			var mockSerializer = new Mock<ISerializer>();
			
			// Act
			new Server("../somepath", mockRestProxy.Object, mockSerializer.Object);
		}

		[Test]
		[Row("ftp://127.0.0.1/somepath")]
		[Row("file:///C:/somepath")]
		[ExpectedArgumentException]
		public void Should_throw_argument_exception_when_url_is_not_using_http_or_https_scheme(string baseUrl)
		{
			// Arrange
			var mockRestProxy = new Mock<IRestProxy>();
			var mockSerializer = new Mock<ISerializer>();
			
			// Act
			new Server(baseUrl, mockRestProxy.Object, mockSerializer.Object);
		}

		[Test]
		[ExpectedArgumentNullException]
		public void Should_throw_argument_null_exception_when_proxy_is_null()
		{
			// Arrange
			string url = "http://127.0.0.1:5984/";
			var mockSerializer = new Mock<ISerializer>();
			
			// Act
			new Server(url, null, mockSerializer.Object);
		}

		[Test]
		[ExpectedArgumentNullException]
		public void Should_throw_argument_null_exception_when_serializer_is_null()
		{
			// Arrange
			string url = "http://127.0.0.1:5984/";
			var mockProxy = new Mock<IRestProxy>();

			// Act
			new Server(url, mockProxy.Object, null);
		}

		[Test]
		[Row("http://127.0.0.1:5984/", "http")]
		[Row("https://domain.com:5984/", "https")]
		public void Should_instantiate_when_given_valid_parameters(string baseUrl, string expectedScheme)
		{
			// Arrange
			var mockRestProxy = new Mock<IRestProxy>();
			var mockSerializer = new Mock<ISerializer>();

			// Act
			IServer server = new Server(baseUrl, mockRestProxy.Object, mockSerializer.Object);

			// Assert
			Assert.IsNotNull(server.Url);
			Assert.AreEqual(baseUrl, server.Url.ToString());
			Assert.AreEqual(expectedScheme, server.Url.Scheme);
			Assert.IsNotNull(server.RestProxy);
			Assert.AreEqual(mockRestProxy.Object, server.RestProxy);
			Assert.IsNotNull(server.Serializer);
			Assert.AreEqual(mockSerializer.Object, server.Serializer);
		}
	}
	
	[TestFixture]
	public class When_creating_a_database
	{
		[Test]
		[Row(null)]
		[Row("")]
		[ExpectedArgumentNullException]
		public void Should_throw_an_argument_null_exception_when_called_with_a_null_or_empty_string(string databaseName)
		{
			// Arrange
			string url = "http://127.0.0.1/";
			var mockRestProxy = new Mock<IRestProxy>();
			var mockSerializer = new Mock<ISerializer>();
			
			// Act
			IServer server = new Server(url, mockRestProxy.Object, mockSerializer.Object);
			server.CreateDatabase(databaseName);
		}

		[Test]
		[Row("test")]
		public void Should_call_put_on_rest_proxy_with_database_name_in_url(string databaseName)
		{
			// Arrange
			string url = "http://127.0.0.1/";
			Uri requestUrl = new Uri(url + databaseName);
			var mockRestProxy = new Mock<IRestProxy>();
			var mockSerializer = new Mock<ISerializer>();
			string body = "{\"ok\":true}";

			mockRestProxy.Setup(x => x.Put(requestUrl)).Returns(new HttpResponse(HttpStatusCode.Created, body));
			
			// Act
			IServer server = new Server(url, mockRestProxy.Object, mockSerializer.Object);
			server.CreateDatabase(databaseName);
			
			// Assert
			mockRestProxy.Verify(x => x.Put(requestUrl), Times.AtLeastOnce());
		}
		
		[Test]
		[Row("test")]
		public void Should_deserialize_error_when_an_error_is_given_in_the_response(string databaseName)
		{
			// Arrange
			string url = "http://127.0.0.1/";
			Uri requestUrl = new Uri(url + databaseName);
			var mockRestProxy = new Mock<IRestProxy>();
			var mockSerializer = new Mock<ISerializer>();
			string body = "{\"error\":\"file_exists\",\"reason\":\"The database could not be created, the file already exists.\"}";

			mockRestProxy.Setup(x => x.Put(requestUrl)).Returns(new HttpResponse(HttpStatusCode.PreconditionFailed, body));
			mockSerializer.Setup(x => x.Deserialize<CouchError>(body)).Returns(new CouchError("file_exists", "The database could not be created, the file already exists."));

			// Act
			try
			{
				IServer server = new Server(url, mockRestProxy.Object, mockSerializer.Object);
				server.CreateDatabase(databaseName);
			}
			catch (CannotCreateDatabaseException)
			{
				// Assert
				mockSerializer.Verify(x => x.Deserialize<CouchError>(body), Times.AtLeastOnce());
			}
		}

		[Test]
		[Row("test")]
		[ExpectedException(typeof(CannotCreateDatabaseException), "Failed to create database 'test'")]
		public void Should_throw_cannot_create_database_exception_when_an_error_is_given_in_the_response(string databaseName)
		{
			// Arrange
			string url = "http://127.0.0.1/";
			Uri requestUrl = new Uri(url + databaseName);
			var mockRestProxy = new Mock<IRestProxy>();
			var mockSerializer = new Mock<ISerializer>();
			string body = "{\"error\":\"file_exists\",\"reason\":\"The database could not be created, the file already exists.\"}";

			mockRestProxy.Setup(x => x.Put(requestUrl)).Returns(new HttpResponse(HttpStatusCode.PreconditionFailed, body));
			mockSerializer.Setup(x => x.Deserialize<CouchError>(body)).Returns(new CouchError("file_exists", "The database could not be created, the file already exists."));

			// Act
			IServer server = new Server(url, mockRestProxy.Object, mockSerializer.Object);
			server.CreateDatabase(databaseName);
		}
	}
	
	[TestFixture]
	public class When_deleting_a_database
	{
		[Test]
		[Row(null)]
		[Row("")]
		[ExpectedArgumentNullException]
		public void Should_throw_an_argument_null_exception_when_called_with_a_null_or_empty_string(string databaseName)
		{
			// Arrange
			string url = "http://127.0.0.1/";
			var mockRestProxy = new Mock<IRestProxy>();
			var mockSerializer = new Mock<ISerializer>();

			// Act
			IServer server = new Server(url, mockRestProxy.Object, mockSerializer.Object);
			server.DeleteDatabase(databaseName);
		}

		[Test]
		[Row("test")]
		public void Should_call_delete_on_rest_proxy_with_database_name_in_the_url(string databaseName)
		{
			// Arrange
			string url = "http://127.0.0.1/";
			Uri requestUrl = new Uri(url + databaseName);
			var mockRestProxy = new Mock<IRestProxy>();
			var mockSerializer = new Mock<ISerializer>();
			string body = "{\"ok\":true}";

			mockRestProxy.Setup(x => x.Delete(requestUrl)).Returns(new HttpResponse(HttpStatusCode.OK, body));

			// Act
			IServer server = new Server(url, mockRestProxy.Object, mockSerializer.Object);
			server.DeleteDatabase(databaseName);

			// Assert
			mockRestProxy.Verify(x => x.Delete(requestUrl), Times.AtLeastOnce());
		}

		[Test]
		[Row("test")]
		public void Should_deserialize_error_when_an_error_is_given_in_the_response(string databaseName)
		{
			// Arrange
			string url = "http://127.0.0.1/";
			Uri requestUrl = new Uri(url + databaseName);
			var mockRestProxy = new Mock<IRestProxy>();
			var mockSerializer = new Mock<ISerializer>();
			string body = "{\"error\":\"not_found\",\"reason\":\"missing\"}";

			mockRestProxy.Setup(x => x.Delete(requestUrl)).Returns(new HttpResponse(HttpStatusCode.NotFound, body));
			mockSerializer.Setup(x => x.Deserialize<CouchError>(body)).Returns(new CouchError("not_found", "missing"));

			// Act
			try
			{
				IServer server = new Server(url, mockRestProxy.Object, mockSerializer.Object);
				server.DeleteDatabase(databaseName);
			}
			catch (CannotDeleteDatabaseException)
			{
				// Assert
				mockSerializer.Verify(x => x.Deserialize<CouchError>(body), Times.AtLeastOnce());
			}
		}

		[Test]
		[Row("test")]
		[ExpectedException(typeof(CannotDeleteDatabaseException), "Failed to delete database 'test'")]
		public void Should_throw_cannot_delete_database_exception_when_an_error_is_given_in_the_response(string databaseName)
		{
			string url = "http://127.0.0.1/";
			Uri requestUrl = new Uri(url + databaseName);
			var mockRestProxy = new Mock<IRestProxy>();
			var mockSerializer = new Mock<ISerializer>();
			string body = "{\"error\":\"not_found\",\"reason\":\"missing\"}";

			mockRestProxy.Setup(x => x.Delete(requestUrl)).Returns(new HttpResponse(HttpStatusCode.NotFound, body));
			mockSerializer.Setup(x => x.Deserialize<CouchError>(body)).Returns(new CouchError("not_found", "missing"));

			IServer server = new Server(url, mockRestProxy.Object, mockSerializer.Object);
			server.DeleteDatabase(databaseName);
		}
	}
	
	[TestFixture]
	public class When_retrieving_a_database
	{
		[Test]
		[Row(null)]
		[Row("")]
		[ExpectedArgumentNullException]
		public void Should_throw_an_argument_null_exception_when_called_with_a_null_or_empty_string(string databaseName)
		{
			// Arrange
			string url = "http://127.0.0.1:5984/";
			var mockRestProxy = new Mock<IRestProxy>();
			var mockSerializer = new Mock<ISerializer>();

			// Act
			IServer server = new Server(url, mockRestProxy.Object, mockSerializer.Object);
			server.GetDatabase(databaseName);
		}

		[Test]
		[Row("test")]
		public void Should_call_get_on_rest_proxy_with_database_name_in_url(string databaseName)
		{
			// Arrange
			string url = "http://127.0.0.1:5984/";
			Uri requestUrl = new Uri(url + databaseName);
			var mockRestProxy = new Mock<IRestProxy>();
			var mockSerializer = new Mock<ISerializer>();
			string body = "{\"db_name\":\"test\",\"doc_count\":0,\"doc_del_count\":0,\"update_seq\":0,\"purge_seq\":0,\"compact_running\":false,\"disk_size\":79,\"instance_start_time\":\"1250175373642458\",\"disk_format_version\":4}";

			mockRestProxy.Setup(x => x.Get(requestUrl)).Returns(new HttpResponse(HttpStatusCode.OK, body));
			mockSerializer.Setup(x => x.Deserialize<DatabaseInfo>(body)).Returns(new DatabaseInfo(databaseName, 0, 0, 0, 0, false, 79, "1250175373642458", 4));

			// Act
			IServer server = new Server(url, mockRestProxy.Object, mockSerializer.Object);
			server.GetDatabase(databaseName);

			// Assert
			mockRestProxy.Verify(x => x.Get(requestUrl), Times.AtLeastOnce());
		}
		
		[Test]
		[Row("test")]
		public void Should_call_deserialize_and_pass_the_body_of_the_response(string databaseName)
		{
			// Arrange
			string url = "http://127.0.0.1:5984/";
			Uri requestUrl = new Uri(url + databaseName);
			var mockRestProxy = new Mock<IRestProxy>();
			var mockSerializer = new Mock<ISerializer>();
			string body = "{\"db_name\":\"test\",\"doc_count\":0,\"doc_del_count\":0,\"update_seq\":0,\"purge_seq\":0,\"compact_running\":false,\"disk_size\":79,\"instance_start_time\":\"1250175373642458\",\"disk_format_version\":4}";

			mockRestProxy.Setup(x => x.Get(requestUrl)).Returns(new HttpResponse(HttpStatusCode.OK, body));
			mockSerializer.Setup(x => x.Deserialize<DatabaseInfo>(body)).Returns(new DatabaseInfo(databaseName, 0, 0, 0, 0, false, 79, "1250175373642458", 4));

			// Act
			IServer server = new Server(url, mockRestProxy.Object, mockSerializer.Object);
			server.GetDatabase(databaseName);

			// Assert
			mockSerializer.Verify(x => x.Deserialize<DatabaseInfo>(body), Times.AtLeastOnce());
		}
		
		[Test]
		[Row("test")]
		public void Should_return_a_new_database_based_on_deserialized_response(string databaseName)
		{
			// Arrange
			string url = "http://127.0.0.1:5984/";
			Uri requestUrl = new Uri(url + databaseName);
			var mockRestProxy = new Mock<IRestProxy>();
			var mockSerializer = new Mock<ISerializer>();
			string body = "{\"db_name\":\"test\",\"doc_count\":0,\"doc_del_count\":0,\"update_seq\":0,\"purge_seq\":0,\"compact_running\":false,\"disk_size\":79,\"instance_start_time\":\"1250175373642458\",\"disk_format_version\":4}";

			mockRestProxy.Setup(x => x.Get(requestUrl)).Returns(new HttpResponse(HttpStatusCode.OK, body));
			mockSerializer.Setup(x => x.Deserialize<DatabaseInfo>(body)).Returns(new DatabaseInfo(databaseName, 0, 0, 0, 0, false, 79, "1250175373642458", 4));

			// Act
			IServer server = new Server(url, mockRestProxy.Object, mockSerializer.Object);
			IDatabase database = server.GetDatabase(databaseName);

			// Assert
			Assert.IsNotNull(database);
			Assert.AreEqual(server, database.Server);
			Assert.AreEqual(databaseName, database.Info.Name);
			Assert.AreEqual(0, database.Info.DocCount);
			Assert.AreEqual(0, database.Info.DocDelCount);
			Assert.AreEqual(0, database.Info.UpdateSequence);
			Assert.AreEqual(0, database.Info.PurgeSequence);
			Assert.AreEqual(false, database.Info.CompactRunning);
			Assert.AreEqual(79, database.Info.DiskSize);
			Assert.AreEqual("1250175373642458", database.Info.InstanceStartTime);
			Assert.AreEqual(4, database.Info.DiskFormatVersion);
		}

		[Test]
		[Row("test")]
		public void Should_deserialize_error_when_an_error_is_given_in_the_response(string databaseName)
		{
			// Arrange
			string url = "http://127.0.0.1:5984/";
			Uri requestUrl = new Uri(url + databaseName);
			var mockRestProxy = new Mock<IRestProxy>();
			var mockSerializer = new Mock<ISerializer>();
			string body = "{\"error\":\"not_found\",\"reason\":\"no_db_file\"}";
			
			mockRestProxy.Setup(x => x.Get(requestUrl)).Returns(new HttpResponse(HttpStatusCode.NotFound, body));
			mockSerializer.Setup(x => x.Deserialize<CouchError>(body)).Returns(new CouchError("not_found", "no_db_file"));

			// Act
			try
			{
				IServer server = new Server(url, mockRestProxy.Object, mockSerializer.Object);
				server.GetDatabase(databaseName);
			}
			catch (Exception)
			{
				mockSerializer.Verify(x => x.Deserialize<CouchError>(body), Times.AtLeastOnce());
			}
		}

		[Test]
		[Row("test")]
		[ExpectedException(typeof(CannotGetDatabaseException), "Failed to get database 'test'")]
		public void Should_throw_cannot_get_database_exception_when_an_error_is_given_in_the_response(string databaseName)
		{
			// Arrange
			string url = "http://127.0.0.1:5984/";
			Uri requestUrl = new Uri(url + databaseName);
			var mockRestProxy = new Mock<IRestProxy>();
			var mockSerializer = new Mock<ISerializer>();
			string body = "{\"error\":\"not_found\",\"reason\":\"no_db_file\"}";
			
			mockRestProxy.Setup(x => x.Get(requestUrl)).Returns(new HttpResponse(HttpStatusCode.NotFound, body));
			mockSerializer.Setup(x => x.Deserialize<CouchError>(body)).Returns(new CouchError("not_found", "no_db_file"));

			// Act
			IServer server = new Server(url, mockRestProxy.Object, mockSerializer.Object);
			server.GetDatabase(databaseName);
		}
	}
	
	[TestFixture]
	public class When_retrieving_a_list_of_databases
	{
		[Test]
		public void Should_call_get_on_rest_proxy_and_pass_all_dbs_on_the_url()
		{
			// Arrange
			string url = "http://127.0.0.1:5984/";
			Uri requestUrl = new Uri(url + "_all_dbs");
			var mockRestProxy = new Mock<IRestProxy>();
			var mockSerializer = new Mock<ISerializer>();
			string body = "[\"test1\",\"test2\"]";
			
			mockRestProxy.Setup(x => x.Get(requestUrl)).Returns(new HttpResponse(HttpStatusCode.OK, body));
			mockSerializer.Setup(x => x.Deserialize<string[]>(body)).Returns(new string[] { "test1", "test2" });
			
			// Act
			IServer server = new Server(url, mockRestProxy.Object, mockSerializer.Object);
			server.GetDatabases();

			// Assert
			mockRestProxy.Verify(x => x.Get(requestUrl));
		}
		
		[Test]
		public void Should_call_deserialize_and_pass_the_body_of_the_response()
		{
			// Arrange
			string url = "http://127.0.0.1:5984/";
			Uri requestUrl = new Uri(url + "_all_dbs");
			var mockRestProxy = new Mock<IRestProxy>();
			var mockSerializer = new Mock<ISerializer>();
			string body = "[\"test1\",\"test2\"]";

			mockRestProxy.Setup(x => x.Get(requestUrl)).Returns(new HttpResponse(HttpStatusCode.OK, body));
			mockSerializer.Setup(x => x.Deserialize<string[]>(body)).Returns(new string[] { "test1", "test2" });

			// Act
			IServer server = new Server(url, mockRestProxy.Object, mockSerializer.Object);
			server.GetDatabases();
			
			// Assert
			mockSerializer.Verify(x => x.Deserialize<string[]>(body));
		}
		
		[Test]
		public void Should_return_a_string_array_populated_with_the_deserialized_response()
		{
			// Arrange
			string url = "http://127.0.0.1:5984/";
			Uri requestUrl = new Uri(url + "_all_dbs");
			var mockRestProxy = new Mock<IRestProxy>();
			var mockSerializer = new Mock<ISerializer>();
			string body = "[\"test1\",\"test2\"]";

			mockRestProxy.Setup(x => x.Get(requestUrl)).Returns(new HttpResponse(HttpStatusCode.OK, body));
			mockSerializer.Setup(x => x.Deserialize<string[]>(body)).Returns(new string[] { "test1", "test2" });
			
			// Act
			IServer server = new Server(url, mockRestProxy.Object, mockSerializer.Object);
			string[] databases = server.GetDatabases();
			
			// Assert
			Assert.IsNotNull(databases);
			Assert.AreEqual(2, databases.Length);
		}
	}
	
	[TestFixture]
	public class When_retrieving_info_about_the_server
	{
		[Test]
		public void Should_call_get_on_rest_proxy_with_the_base_url()
		{
			// Arrange
			string url = "http://127.0.0.1:5984/";
			Uri requestUrl = new Uri(url);
			var mockRestProxy = new Mock<IRestProxy>();
			var mockSerializer = new Mock<ISerializer>();
			string body = "{\"couchdb\":\"Welcome\",\"version\":\"0.10.0a800465\"}";

			mockRestProxy.Setup(x => x.Get(requestUrl)).Returns(new HttpResponse(HttpStatusCode.OK, body));
			mockSerializer.Setup(x => x.Deserialize<ServerInfo>(body)).Returns(new ServerInfo("Welcome", "0.10.0a800465"));

			// Act
			IServer server = new Server(url, mockRestProxy.Object, mockSerializer.Object);
			server.GetInfo();

			// Assert
			mockRestProxy.Verify(x => x.Get(requestUrl));
		}
		
		[Test]
		public void Should_call_deserialize_and_pass_the_body_of_the_response()
		{
			// Arrange
			string url = "http://127.0.0.1:5984/";
			Uri requestUrl = new Uri(url);
			var mockRestProxy = new Mock<IRestProxy>();
			var mockSerializer = new Mock<ISerializer>();
			string body = "{\"couchdb\":\"Welcome\",\"version\":\"0.10.0a800465\"}";

			mockRestProxy.Setup(x => x.Get(requestUrl)).Returns(new HttpResponse(HttpStatusCode.OK, body));
			mockSerializer.Setup(x => x.Deserialize<ServerInfo>(body)).Returns(new ServerInfo("Welcome", "0.10.0a800465"));

			// Act
			IServer server = new Server(url, mockRestProxy.Object, mockSerializer.Object);
			server.GetInfo();

			// Assert
			mockSerializer.Verify(x => x.Deserialize<ServerInfo>(body));
		}
		
		[Test]
		public void Should_return_a_new_server_info_instance_populated_with_the_deserialized_response()
		{
			// Arrange
			string url = "http://127.0.0.1:5984/";
			Uri requestUrl = new Uri(url);
			var mockRestProxy = new Mock<IRestProxy>();
			var mockSerializer = new Mock<ISerializer>();
			string body = "{\"couchdb\":\"Welcome\",\"version\":\"0.10.0a800465\"}";

			mockRestProxy.Setup(x => x.Get(requestUrl)).Returns(new HttpResponse(HttpStatusCode.OK, body));
			mockSerializer.Setup(x => x.Deserialize<ServerInfo>(body)).Returns(new ServerInfo("Welcome", "0.10.0a800465"));

			// Act
			IServer server = new Server(url, mockRestProxy.Object, mockSerializer.Object);
			IServerInfo serverInfo = server.GetInfo();

			// Assert
			Assert.IsNotNull(serverInfo);
			Assert.AreEqual("Welcome", serverInfo.Message);
			Assert.AreEqual("0.10.0a800465", serverInfo.Version);
		}
	}
}