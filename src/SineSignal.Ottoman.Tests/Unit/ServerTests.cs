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

using SineSignal.Ottoman.Exceptions;
using SineSignal.Ottoman.Model;
using SineSignal.Ottoman.Proxy;
using SineSignal.Ottoman.Serializers;

namespace SineSignal.Ottoman.Tests.Unit
{
	[TestFixture]
	[Category("Unit")]
	public class When_connecting : OttomanSpecBase<Server>
	{
		[Test]
		[Row(null)]
		[Row("")]
		[ExpectedArgumentNullException]
		public void Should_throw_argument_null_exception_when_url_is_null_or_empty_string(string baseAddress)
		{
			Server.Connect(baseAddress);
		}

		[Test]
		[ExpectedException(typeof(UriFormatException), "The value is invalid, please pass a valid Uri.")]
		public void Should_throw_uri_format_exception_when_url_is_a_random_string()
		{
			Server.Connect("some string value");
		}

		[Test]
		[ExpectedException(typeof(UriFormatException), "The value is invalid, please pass a valid Uri.")]
		public void Should_throw_uri_format_exception_when_url_is_relative()
		{
			Server.Connect("../somepath");
		}

		[Test]
		[Row("ftp://127.0.0.1/somepath")]
		[Row("file:///C:/somepath")]
		[ExpectedArgumentException]
		public void Should_throw_argument_exception_when_url_is_not_using_http_or_https_scheme(string baseAddress)
		{
			Server.Connect(baseAddress);
		}

		[Test]
		[Row("http://127.0.0.1:5984/")]
		[Row("https://domain.com:5984/")]
		public void Should_set_Address_when_given_an_address(string address)
		{
			var parsedAddress = new Uri(address);
			var fakeRestClient = new Mock<IRestClient>();
			var fakeSerializer = new Mock<ISerializer>();

			IServer server = new Server(parsedAddress, fakeRestClient.Object, fakeSerializer.Object);

			Assert.AreEqual(parsedAddress, server.Address);
		}

		[Test]
		[Row("http://127.0.0.1:5984/")]
		[Row("https://domain.com:5984/")]
		public void Should_set_RestClient_when_given_an_implementation_of_IRestClient(string address)
		{
			var parsedAddress = new Uri(address);
			var fakeRestClient = new Mock<IRestClient>();
			var fakeSerializer = new Mock<ISerializer>();

			IServer server = new Server(parsedAddress, fakeRestClient.Object, fakeSerializer.Object);

			Assert.AreEqual(fakeRestClient.Object, server.RestClient);
		}

		[Test]
		[Row("http://127.0.0.1:5984/")]
		[Row("https://domain.com:5984/")]
		public void Should_set_Serializer_when_given_an_implemenation_of_ISerializer(string address)
		{
			var parsedAddress = new Uri(address);
			var fakeRestClient = new Mock<IRestClient>();
			var fakeSerializer = new Mock<ISerializer>();

			IServer server = new Server(parsedAddress, fakeRestClient.Object, fakeSerializer.Object);

			Assert.AreEqual(fakeSerializer.Object, server.Serializer);
		}
		
		[Test]
		public void Should_set_Address_to_default()
		{
			Uri defaultAddress = new Uri("http://127.0.0.1:5984/");
			
			IServer server = Server.Connect();

			Assert.AreEqual(defaultAddress, server.Address);
		}
	}
	
	[TestFixture]
	[Category("Unit")]
	public class When_creating_a_database : OttomanSpecBase<Server>
	{
		private Uri Url { get; set; }
		private Mock<IRestClient> FakeRestClient { get; set; }
		private Mock<ISerializer> FakeSerializer { get; set; }
		
		protected override Server EstablishContext()
		{
			Url = new Uri("http://127.0.0.1:5984/");
			FakeRestClient = new Mock<IRestClient>();
			FakeSerializer = new Mock<ISerializer>();

			return new Server(Url, FakeRestClient.Object, FakeSerializer.Object);
		}

		[Test]
		[Row(null)]
		[Row("")]
		[ExpectedArgumentNullException]
		public void Should_throw_an_argument_null_exception_when_called_with_a_null_or_empty_string(string databaseName)
		{
			Sut.CreateDatabase(databaseName);
		}

		[Test]
		[Row("test")]
		public void Should_call_put_on_RestClient_with_database_name_in_url(string databaseName)
		{
			Uri requestUrl = new Uri(Url + databaseName);
			string body = "{\"ok\":true}";

			FakeRestClient.Setup(x => x.Put(requestUrl)).Returns(new HttpResponse(HttpStatusCode.Created, body));
			
			Sut.CreateDatabase(databaseName);
			
			FakeRestClient.Verify(x => x.Put(requestUrl), Times.AtLeastOnce());
		}
		
		[Test]
		[Row("test")]
		public void Should_deserialize_error_when_an_error_is_given_in_the_response(string databaseName)
		{
			Uri requestUrl = new Uri(Url + databaseName);
			string body = "{\"error\":\"file_exists\",\"reason\":\"The database could not be created, the file already exists.\"}";

			FakeRestClient.Setup(x => x.Put(requestUrl)).Returns(new HttpResponse(HttpStatusCode.PreconditionFailed, body));
			FakeSerializer.Setup(x => x.Deserialize<ErrorInfo>(body)).Returns(new ErrorInfo("file_exists", "The database could not be created, the file already exists."));

			try
			{
				Sut.CreateDatabase(databaseName);
			}
			catch (CannotCreateDatabaseException)
			{
				FakeSerializer.Verify(x => x.Deserialize<ErrorInfo>(body), Times.AtLeastOnce());
			}
		}

		[Test]
		[Row("test")]
		[ExpectedException(typeof(CannotCreateDatabaseException), "Failed to create database 'test'")]
		public void Should_throw_cannot_create_database_exception_when_an_error_is_given_in_the_response(string databaseName)
		{
			Uri requestUrl = new Uri(Url + databaseName);
			string body = "{\"error\":\"file_exists\",\"reason\":\"The database could not be created, the file already exists.\"}";

			FakeRestClient.Setup(x => x.Put(requestUrl)).Returns(new HttpResponse(HttpStatusCode.PreconditionFailed, body));
			FakeSerializer.Setup(x => x.Deserialize<ErrorInfo>(body)).Returns(new ErrorInfo("file_exists", "The database could not be created, the file already exists."));

			Sut.CreateDatabase(databaseName);
		}
	}
	
	[TestFixture]
	[Category("Unit")]
	public class When_deleting_a_database : OttomanSpecBase<Server>
	{
		private Uri Url { get; set; }
		private Mock<IRestClient> FakeRestClient { get; set; }
		private Mock<ISerializer> FakeSerializer { get; set; }

		protected override Server EstablishContext()
		{
			Url = new Uri("http://127.0.0.1:5984/");
			FakeRestClient = new Mock<IRestClient>();
			FakeSerializer = new Mock<ISerializer>();

			return new Server(Url, FakeRestClient.Object, FakeSerializer.Object);
		}
		
		[Test]
		[Row(null)]
		[Row("")]
		[ExpectedArgumentNullException]
		public void Should_throw_an_argument_null_exception_when_called_with_a_null_or_empty_string(string databaseName)
		{
			Sut.DeleteDatabase(databaseName);
		}

		[Test]
		[Row("test")]
		public void Should_call_delete_on_RestClient_with_database_name_in_the_url(string databaseName)
		{
			Uri requestUrl = new Uri(Url + databaseName);
			string body = "{\"ok\":true}";

			FakeRestClient.Setup(x => x.Delete(requestUrl)).Returns(new HttpResponse(HttpStatusCode.OK, body));

			Sut.DeleteDatabase(databaseName);

			FakeRestClient.Verify(x => x.Delete(requestUrl), Times.AtLeastOnce());
		}

		[Test]
		[Row("test")]
		public void Should_deserialize_error_when_an_error_is_given_in_the_response(string databaseName)
		{
			Uri requestUrl = new Uri(Url + databaseName);
			string body = "{\"error\":\"not_found\",\"reason\":\"missing\"}";

			FakeRestClient.Setup(x => x.Delete(requestUrl)).Returns(new HttpResponse(HttpStatusCode.NotFound, body));
			FakeSerializer.Setup(x => x.Deserialize<ErrorInfo>(body)).Returns(new ErrorInfo("not_found", "missing"));

			try
			{
				Sut.DeleteDatabase(databaseName);
			}
			catch (CannotDeleteDatabaseException)
			{
				FakeSerializer.Verify(x => x.Deserialize<ErrorInfo>(body), Times.AtLeastOnce());
			}
		}

		[Test]
		[Row("test")]
		[ExpectedException(typeof(CannotDeleteDatabaseException), "Failed to delete database 'test'")]
		public void Should_throw_cannot_delete_database_exception_when_an_error_is_given_in_the_response(string databaseName)
		{
			Uri requestUrl = new Uri(Url + databaseName);
			string body = "{\"error\":\"not_found\",\"reason\":\"missing\"}";

			FakeRestClient.Setup(x => x.Delete(requestUrl)).Returns(new HttpResponse(HttpStatusCode.NotFound, body));
			FakeSerializer.Setup(x => x.Deserialize<ErrorInfo>(body)).Returns(new ErrorInfo("not_found", "missing"));

			Sut.DeleteDatabase(databaseName);
		}
	}
	
	[TestFixture]
	[Category("Unit")]
	public class When_retrieving_a_database : OttomanSpecBase<Server>
	{
		private Uri Url { get; set; }
		private Mock<IRestClient> FakeRestClient { get; set; }
		private Mock<ISerializer> FakeSerializer { get; set; }

		protected override Server EstablishContext()
		{
			Url = new Uri("http://127.0.0.1:5984/");
			FakeRestClient = new Mock<IRestClient>();
			FakeSerializer = new Mock<ISerializer>();

			return new Server(Url, FakeRestClient.Object, FakeSerializer.Object);
		}
		
		[Test]
		[Row(null)]
		[Row("")]
		[ExpectedArgumentNullException]
		public void Should_throw_an_argument_null_exception_when_called_with_a_null_or_empty_string(string databaseName)
		{
			Sut.GetDatabase(databaseName);
		}

		[Test]
		[Row("test")]
		public void Should_call_get_on_RestClient_with_database_name_in_url(string databaseName)
		{
			Uri requestUrl = new Uri(Url + databaseName);
			string body = "{\"db_name\":\"test\",\"doc_count\":0,\"doc_del_count\":0,\"update_seq\":0,\"purge_seq\":0,\"compact_running\":false,\"disk_size\":79,\"instance_start_time\":\"1250175373642458\",\"disk_format_version\":4}";

			FakeRestClient.Setup(x => x.Get(requestUrl)).Returns(new HttpResponse(HttpStatusCode.OK, body));
			FakeSerializer.Setup(x => x.Deserialize<DatabaseInfo>(body)).Returns(new DatabaseInfo(databaseName, 0, 0, 0, 0, false, 79, "1250175373642458", 4));

			Sut.GetDatabase(databaseName);

			FakeRestClient.Verify(x => x.Get(requestUrl), Times.AtLeastOnce());
		}
		
		[Test]
		[Row("test")]
		public void Should_call_deserialize_and_pass_the_body_of_the_response(string databaseName)
		{
			Uri requestUrl = new Uri(Url + databaseName);
			string body = "{\"db_name\":\"test\",\"doc_count\":0,\"doc_del_count\":0,\"update_seq\":0,\"purge_seq\":0,\"compact_running\":false,\"disk_size\":79,\"instance_start_time\":\"1250175373642458\",\"disk_format_version\":4}";

			FakeRestClient.Setup(x => x.Get(requestUrl)).Returns(new HttpResponse(HttpStatusCode.OK, body));
			FakeSerializer.Setup(x => x.Deserialize<DatabaseInfo>(body)).Returns(new DatabaseInfo(databaseName, 0, 0, 0, 0, false, 79, "1250175373642458", 4));

			Sut.GetDatabase(databaseName);

			FakeSerializer.Verify(x => x.Deserialize<DatabaseInfo>(body), Times.AtLeastOnce());
		}
		
		[Test]
		[Row("test")]
		public void Should_return_a_new_database_based_on_deserialized_response(string databaseName)
		{
			Uri requestUrl = new Uri(Url + databaseName);
			string body = "{\"db_name\":\"test\",\"doc_count\":0,\"doc_del_count\":0,\"update_seq\":0,\"purge_seq\":0,\"compact_running\":false,\"disk_size\":79,\"instance_start_time\":\"1250175373642458\",\"disk_format_version\":4}";

			FakeRestClient.Setup(x => x.Get(requestUrl)).Returns(new HttpResponse(HttpStatusCode.OK, body));
			FakeSerializer.Setup(x => x.Deserialize<DatabaseInfo>(body)).Returns(new DatabaseInfo(databaseName, 0, 0, 0, 0, false, 79, "1250175373642458", 4));

			IDatabase database = Sut.GetDatabase(databaseName);

			Assert.AreEqual(Sut, database.Server);
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
			Uri requestUrl = new Uri(Url + databaseName);
			string body = "{\"error\":\"not_found\",\"reason\":\"no_db_file\"}";
			
			FakeRestClient.Setup(x => x.Get(requestUrl)).Returns(new HttpResponse(HttpStatusCode.NotFound, body));
			FakeSerializer.Setup(x => x.Deserialize<ErrorInfo>(body)).Returns(new ErrorInfo("not_found", "no_db_file"));

			try
			{
				Sut.GetDatabase(databaseName);
			}
			catch (Exception)
			{
				FakeSerializer.Verify(x => x.Deserialize<ErrorInfo>(body), Times.AtLeastOnce());
			}
		}

		[Test]
		[Row("test")]
		[ExpectedException(typeof(CannotGetDatabaseException), "Failed to get database 'test'")]
		public void Should_throw_cannot_get_database_exception_when_an_error_is_given_in_the_response(string databaseName)
		{
			Uri requestUrl = new Uri(Url + databaseName);
			string body = "{\"error\":\"not_found\",\"reason\":\"no_db_file\"}";
			
			FakeRestClient.Setup(x => x.Get(requestUrl)).Returns(new HttpResponse(HttpStatusCode.NotFound, body));
			FakeSerializer.Setup(x => x.Deserialize<ErrorInfo>(body)).Returns(new ErrorInfo("not_found", "no_db_file"));

			Sut.GetDatabase(databaseName);
		}
	}
	
	[TestFixture]
	[Category("Unit")]
	public class When_retrieving_a_list_of_databases : OttomanSpecBase<Server>
	{
		private Uri Url { get; set; }
		private Uri RequestUrl { get; set; }
		private Mock<IRestClient> FakeRestClient { get; set; }
		private Mock<ISerializer> FakeSerializer { get; set; }

		protected override Server EstablishContext()
		{
			Url = new Uri("http://127.0.0.1:5984/");
			RequestUrl = new Uri(Url + "_all_dbs");
			FakeRestClient = new Mock<IRestClient>();
			FakeSerializer = new Mock<ISerializer>();

			return new Server(Url, FakeRestClient.Object, FakeSerializer.Object);
		}
		
		[Test]
		public void Should_call_get_on_RestClient_and_pass_all_dbs_on_the_url()
		{
			string body = "[\"test1\",\"test2\"]";
			
			FakeRestClient.Setup(x => x.Get(RequestUrl)).Returns(new HttpResponse(HttpStatusCode.OK, body));
			FakeSerializer.Setup(x => x.Deserialize<string[]>(body)).Returns(new string[] { "test1", "test2" });
			
			Sut.GetDatabases();

			FakeRestClient.Verify(x => x.Get(RequestUrl));
		}
		
		[Test]
		public void Should_call_deserialize_and_pass_the_body_of_the_response()
		{
			string body = "[\"test1\",\"test2\"]";

			FakeRestClient.Setup(x => x.Get(RequestUrl)).Returns(new HttpResponse(HttpStatusCode.OK, body));
			FakeSerializer.Setup(x => x.Deserialize<string[]>(body)).Returns(new string[] { "test1", "test2" });

			Sut.GetDatabases();
			
			FakeSerializer.Verify(x => x.Deserialize<string[]>(body));
		}
		
		[Test]
		public void Should_return_a_string_array_of_database_names_populated_with_the_deserialized_response()
		{
			string body = "[\"test1\",\"test2\"]";

			FakeRestClient.Setup(x => x.Get(RequestUrl)).Returns(new HttpResponse(HttpStatusCode.OK, body));
			FakeSerializer.Setup(x => x.Deserialize<string[]>(body)).Returns(new string[] { "test1", "test2" });
			
			string[] databases = Sut.GetDatabases();
			
			Assert.AreEqual(2, databases.Length);
			Assert.AreEqual("test1", databases[0]);
			Assert.AreEqual("test2", databases[1]);
		}
	}
	
	[TestFixture]
	[Category("Unit")]
	public class When_retrieving_info_about_the_server : OttomanSpecBase<Server>
	{
		private Uri Url { get; set; }
		private Mock<IRestClient> FakeRestClient { get; set; }
		private Mock<ISerializer> FakeSerializer { get; set; }

		protected override Server EstablishContext()
		{
			Url = new Uri("http://127.0.0.1:5984/");
			FakeRestClient = new Mock<IRestClient>();
			FakeSerializer = new Mock<ISerializer>();

			return new Server(Url, FakeRestClient.Object, FakeSerializer.Object);
		}
		
		[Test]
		public void Should_call_get_on_RestClient_with_the_base_url()
		{
			string body = "{\"couchdb\":\"Welcome\",\"version\":\"0.10.0a800465\"}";

			FakeRestClient.Setup(x => x.Get(Url)).Returns(new HttpResponse(HttpStatusCode.OK, body));
			FakeSerializer.Setup(x => x.Deserialize<ServerInfo>(body)).Returns(new ServerInfo("Welcome", "0.10.0a800465"));

			Sut.GetInfo();

			FakeRestClient.Verify(x => x.Get(Url));
		}
		
		[Test]
		public void Should_call_deserialize_and_pass_the_body_of_the_response()
		{
			string body = "{\"couchdb\":\"Welcome\",\"version\":\"0.10.0a800465\"}";

			FakeRestClient.Setup(x => x.Get(Url)).Returns(new HttpResponse(HttpStatusCode.OK, body));
			FakeSerializer.Setup(x => x.Deserialize<ServerInfo>(body)).Returns(new ServerInfo("Welcome", "0.10.0a800465"));

			Sut.GetInfo();

			FakeSerializer.Verify(x => x.Deserialize<ServerInfo>(body));
		}
		
		[Test]
		public void Should_return_a_new_server_info_instance_populated_with_the_deserialized_response()
		{
			string body = "{\"couchdb\":\"Welcome\",\"version\":\"0.10.0a800465\"}";

			FakeRestClient.Setup(x => x.Get(Url)).Returns(new HttpResponse(HttpStatusCode.OK, body));
			FakeSerializer.Setup(x => x.Deserialize<ServerInfo>(body)).Returns(new ServerInfo("Welcome", "0.10.0a800465"));

			IServerInfo serverInfo = Sut.GetInfo();

			Assert.AreEqual("Welcome", serverInfo.Message);
			Assert.AreEqual("0.10.0a800465", serverInfo.Version);
		}
	}

	[TestFixture]
	[Category("Unit")]
	public class When_retrieving_a_list_of_uuids : OttomanSpecBase<Server>
	{
		private Uri Url { get; set; }
		private Mock<IRestClient> FakeRestClient { get; set; }
		private Mock<ISerializer> FakeSerializer { get; set; }

		protected override Server EstablishContext()
		{
			Url = new Uri("http://127.0.0.1:5984/");
			FakeRestClient = new Mock<IRestClient>();
			FakeSerializer = new Mock<ISerializer>();

			return new Server(Url, FakeRestClient.Object, FakeSerializer.Object);
		}

		[Test]
		public void Should_call_get_on_RestClient_and_pass_uuids_and_count_parameter_on_the_url()
		{
			int count = 2;
			Uri requestUrl = new Uri(Url + "_uuids?count=" + count);
			string body = "[\"473b6a153627bdd551a712ac09abe847\",\"9df864f019a8a0e7435ff29a45205a71\"]";

			FakeRestClient.Setup(x => x.Get(requestUrl)).Returns(new HttpResponse(HttpStatusCode.OK, body));
			FakeSerializer.Setup(x => x.Deserialize<UuidInfo>(body)).Returns(new UuidInfo(new Guid[] { new Guid("473b6a153627bdd551a712ac09abe847"), new Guid("9df864f019a8a0e7435ff29a45205a71") }));

			Sut.GetUuids(count);

			FakeRestClient.Verify(x => x.Get(requestUrl));
		}

		[Test]
		public void Should_call_deserialize_and_pass_the_body_of_the_response()
		{
			int count = 2;
			Uri requestUrl = new Uri(Url + "_uuids?count=" + count);
			string body = "[\"473b6a153627bdd551a712ac09abe847\",\"9df864f019a8a0e7435ff29a45205a71\"]";

			FakeRestClient.Setup(x => x.Get(requestUrl)).Returns(new HttpResponse(HttpStatusCode.OK, body));
			FakeSerializer.Setup(x => x.Deserialize<UuidInfo>(body)).Returns(new UuidInfo(new Guid[] { new Guid("473b6a153627bdd551a712ac09abe847"), new Guid("9df864f019a8a0e7435ff29a45205a71") }));

			Sut.GetUuids(count);

			FakeSerializer.Verify(x => x.Deserialize<UuidInfo>(body));
		}

		[Test]
		public void Should_return_a_string_array_of_uuids_populated_with_the_deserialized_response()
		{
			int count = 2;
			Uri requestUrl = new Uri(Url + "_uuids?count=" + count);
			string body = "[\"473b6a153627bdd551a712ac09abe847\",\"9df864f019a8a0e7435ff29a45205a71\"]";

			FakeRestClient.Setup(x => x.Get(requestUrl)).Returns(new HttpResponse(HttpStatusCode.OK, body));
			FakeSerializer.Setup(x => x.Deserialize<UuidInfo>(body)).Returns(new UuidInfo(new Guid[] { new Guid("473b6a153627bdd551a712ac09abe847"), new Guid("9df864f019a8a0e7435ff29a45205a71") }));

			Guid[] uuids = Sut.GetUuids(count);

			Assert.AreEqual(count, uuids.Length);
			Assert.AreEqual(new Guid("473b6a153627bdd551a712ac09abe847"), uuids[0]);
			Assert.AreEqual(new Guid("9df864f019a8a0e7435ff29a45205a71"), uuids[1]);
		}
	}
}