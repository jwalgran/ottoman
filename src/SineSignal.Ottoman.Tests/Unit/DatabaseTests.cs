#region License

// <copyright file="DatabaseTests.cs" company="SineSignal, LLC.">
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

using SineSignal.Ottoman.Model;
using SineSignal.Ottoman.Proxy;
using SineSignal.Ottoman.Serializers;
using SineSignal.Ottoman.Tests.SampleDomain;

namespace SineSignal.Ottoman.Tests.Unit
{
	[TestFixture]
	[Category("Unit")]
	public class When_updating_the_database_info : OttomanSpecBase<Database>
	{
		private string Url { get; set; }
		private string DatabaseName { get; set; }
		private Uri RequestUrl { get; set; }
		private Mock<IServer> FakeServer { get; set; }
		private Mock<IDatabaseInfo> FakeDatabaseInfo { get; set; }
		private string Body { get; set; }
		private Mock<IRestClient> FakeRestClient { get; set; }
		private Mock<ISerializer> FakeSerializer { get; set; }
		
		protected override Database EstablishContext()
		{
			Url = "http://127.0.0.1:5984/";
			DatabaseName = "test";
			RequestUrl = new Uri(Url + DatabaseName);
			
			FakeServer = new Mock<IServer>();
			FakeServer.SetupGet(x => x.Address).Returns(new Uri(Url));

			FakeDatabaseInfo = new Mock<IDatabaseInfo>();
			FakeDatabaseInfo.SetupGet(x => x.Name).Returns(DatabaseName);
			
			Body = "{\"db_name\":\"" + DatabaseName + "\",\"doc_count\":0,\"doc_del_count\":0,\"update_seq\":0,\"purge_seq\":0,\"compact_running\":false,\"disk_size\":79,\"instance_start_time\":\"1250175373642458\",\"disk_format_version\":4}";
			FakeRestClient = new Mock<IRestClient>();
			FakeRestClient.Setup(x => x.Get(RequestUrl)).Returns(new HttpResponse(HttpStatusCode.OK, Body));

			FakeSerializer = new Mock<ISerializer>();
			FakeSerializer.Setup(x => x.Deserialize<DatabaseInfo>(Body)).Returns(new DatabaseInfo(DatabaseName, 0, 0, 0, 0, false, 79, "1250175373642458", 4));
			
			return new Database(FakeServer.Object, FakeDatabaseInfo.Object, FakeRestClient.Object, FakeSerializer.Object);
		}
		
		[Test]
		public void Should_leverage_Server_to_retrieve_base_url()
		{
			Sut.UpdateInfo();
			
			FakeServer.VerifyGet(x => x.Address);
		}

		[Test]
		public void Should_leverage_DatabaseInfo_to_retrieve_database_name()
		{
			Sut.UpdateInfo();

			FakeDatabaseInfo.VerifyGet(x => x.Name);
		}
		
		[Test]
		[Row("test")]
		public void Should_call_get_on_RestProxy_passing_the_database_name_on_the_url()
		{
			Sut.UpdateInfo();
			
			FakeRestClient.Verify(x => x.Get(RequestUrl));
		}
		
		[Test]
		public void Should_call_deserialize_on_Serializer_passing_the_response_body()
		{
			Sut.UpdateInfo();
			
			FakeSerializer.Verify(x => x.Deserialize<DatabaseInfo>(Body));
		}
		
		[Test]
		public void Should_set_DatabaseInfo_from_deserialized_body()
		{
			Sut.UpdateInfo();
			
			Assert.AreEqual(DatabaseName, Sut.Info.Name);
			Assert.AreEqual(0, Sut.Info.DocCount);
			Assert.AreEqual(0, Sut.Info.DocDelCount);
			Assert.AreEqual(0, Sut.Info.UpdateSequence);
			Assert.AreEqual(0, Sut.Info.PurgeSequence);
			Assert.AreEqual(false, Sut.Info.CompactRunning);
			Assert.AreEqual(79, Sut.Info.DiskSize);
			Assert.AreEqual("1250175373642458", Sut.Info.InstanceStartTime);
			Assert.AreEqual(4, Sut.Info.DiskFormatVersion);
		}
	}
	
	[TestFixture]
	[Category("Unit")]
	public class When_massaging_json_for_sending : OttomanSpecBase<Database>
	{
		private Mock<IServer> FakeServer { get; set; }
		private Mock<IDatabaseInfo> FakeDatabaseInfo { get; set; }
		private Mock<IRestClient> FakeRestClient { get; set; }
		private Mock<ISerializer> FakeSerializer { get; set; }
		
		protected override Database EstablishContext()
		{
			FakeServer = new Mock<IServer>();
			FakeDatabaseInfo = new Mock<IDatabaseInfo>();
			FakeRestClient = new Mock<IRestClient>();
			FakeSerializer = new Mock<ISerializer>();
			
			return new Database(FakeServer.Object, FakeDatabaseInfo.Object, FakeRestClient.Object, FakeSerializer.Object);
		}

		public void Should_prep_json_by_removing_id_and_adding_doc_type()
		{
			string json = "{\"Id\":\"fe875b98-0ef2-42c2-9c7f-94ab94432250\",\"Subordinates\":[{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":40.0,\"Id\":\"4c5b075c-b87e-46b9-9108-6dd3a647953b\",\"Name\":\"Bob\",\"Login\":\"bbob\"},{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":40.0,\"Id\":\"b14818db-c975-4109-a94a-452632ee161b\",\"Name\":\"Alice\",\"Login\":\"aalice\"},{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":20.0,\"Id\":\"8f0a0036-319f-49e6-83e7-f84971f9aa5c\",\"Name\":\"Eve\",\"Login\":\"eeve\"}],\"Name\":\"Chris\",\"Login\":\"cchandler\"}";
			string jsonMinusId = "{\"Subordinates\":[{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":40.0,\"Id\":\"4c5b075c-b87e-46b9-9108-6dd3a647953b\",\"Name\":\"Bob\",\"Login\":\"bbob\"},{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":40.0,\"Id\":\"b14818db-c975-4109-a94a-452632ee161b\",\"Name\":\"Alice\",\"Login\":\"aalice\"},{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":20.0,\"Id\":\"8f0a0036-319f-49e6-83e7-f84971f9aa5c\",\"Name\":\"Eve\",\"Login\":\"eeve\"}],\"Name\":\"Chris\",\"Login\":\"cchandler\"}";
			string jsonMassaged = "{\"Subordinates\":[{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":40.0,\"Id\":\"4c5b075c-b87e-46b9-9108-6dd3a647953b\",\"Name\":\"Bob\",\"Login\":\"bbob\"},{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":40.0,\"Id\":\"b14818db-c975-4109-a94a-452632ee161b\",\"Name\":\"Alice\",\"Login\":\"aalice\"},{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":20.0,\"Id\":\"8f0a0036-319f-49e6-83e7-f84971f9aa5c\",\"Name\":\"Eve\",\"Login\":\"eeve\"}],\"Name\":\"Chris\",\"Login\":\"cchandler\",\"doc_type\":\"DocType\"}";

			FakeSerializer.Setup(x => x.RemoveKeyFrom(json, "Id"));
			FakeSerializer.Setup(x => x.AddKeyTo(jsonMinusId, "doc_type", "DocType"));
			
			string result = Sut.MassageJsonForSending(json, "DocType");
			
			FakeSerializer.Verify(x => x.RemoveKeyFrom(json, "Id"));
			FakeSerializer.Verify(x => x.AddKeyTo(jsonMinusId, "doc_type", "DocType"));
			Assert.AreEqual(jsonMassaged, result);
		}
	}

	[TestFixture]
	[Category("Unit")]
	public class DatabaseTests
	{
		/* 
		 * Now it's time for the interesting functionality of Ottoman.  Making it as seamless 
		 * as possible to create, retrieve, update, and delete documents.
		 *		Let's start with creating, since we can't do the others without creating first.
		 *			Tasks for creating a document:
		 *				Automatically generate ID and assign it to the object.
		 *				Assigns a doc_type to the document, based on the type of the object passed in.
		 *				Serializes the object to JSON.
		 *				PUTS the JSON to the CouchDB server.
		 *				Verifies the POST and returns the object, otherwise throw exception.
		 */
		 [Test]
		 [Row("test")]
		 public void Should_be_able_to_create_document_when_given_an_object(string databaseName)
		 {
			string url = "http://127.0.0.1:5984/";
			UriBuilder requestUrl = new UriBuilder(url);
			requestUrl.Path = databaseName + "/" + "fe875b98-0ef2-42c2-9c7f-94ab94432250";
		 	var manager = Manager.CreateManager();
		 	Type type = typeof (Manager);
		 	string docType = type.Name;
		 	string json = "{\"Id\":\"fe875b98-0ef2-42c2-9c7f-94ab94432250\",\"Subordinates\":[{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":40.0,\"Id\":\"4c5b075c-b87e-46b9-9108-6dd3a647953b\",\"Name\":\"Bob\",\"Login\":\"bbob\"},{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":40.0,\"Id\":\"b14818db-c975-4109-a94a-452632ee161b\",\"Name\":\"Alice\",\"Login\":\"aalice\"},{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":20.0,\"Id\":\"8f0a0036-319f-49e6-83e7-f84971f9aa5c\",\"Name\":\"Eve\",\"Login\":\"eeve\"}],\"Name\":\"Chris\",\"Login\":\"cchandler\"}";
			string jsonMinusId = "{\"Subordinates\":[{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":40.0,\"Id\":\"4c5b075c-b87e-46b9-9108-6dd3a647953b\",\"Name\":\"Bob\",\"Login\":\"bbob\"},{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":40.0,\"Id\":\"b14818db-c975-4109-a94a-452632ee161b\",\"Name\":\"Alice\",\"Login\":\"aalice\"},{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":20.0,\"Id\":\"8f0a0036-319f-49e6-83e7-f84971f9aa5c\",\"Name\":\"Eve\",\"Login\":\"eeve\"}],\"Name\":\"Chris\",\"Login\":\"cchandler\"}";
		 	string jsonDocTypeAdded = "{\"Subordinates\":[{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":40.0,\"Id\":\"4c5b075c-b87e-46b9-9108-6dd3a647953b\",\"Name\":\"Bob\",\"Login\":\"bbob\"},{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":40.0,\"Id\":\"b14818db-c975-4109-a94a-452632ee161b\",\"Name\":\"Alice\",\"Login\":\"aalice\"},{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":20.0,\"Id\":\"8f0a0036-319f-49e6-83e7-f84971f9aa5c\",\"Name\":\"Eve\",\"Login\":\"eeve\"}],\"Name\":\"Chris\",\"Login\":\"cchandler\",\"doc_type\":\"" + docType + "\"}";
		 	
		 	var fakeServer = new Mock<IServer>();
		 	var fakeDatabaseInfo = new Mock<IDatabaseInfo>();
		 	var fakeSerializer = new Mock<ISerializer>();
		 	var fakeRestClient = new Mock<IRestClient>();
		 	var fakeHttpResponse = new Mock<IHttpResponse>();
		 	var fakeDocument = new Mock<IDocument>();

		 	fakeSerializer.Setup(x => x.Serialize(manager)).Returns(json);
		 	fakeSerializer.Setup(x => x.RemoveKeyFrom(json, "Id")).Returns(jsonMinusId);
		 	fakeSerializer.Setup(x => x.AddKeyTo(jsonMinusId, "doc_type", docType)).Returns(jsonDocTypeAdded);
		 	fakeSerializer.Setup(x => x.ContentType).Returns("application/json");
			fakeRestClient.Setup(x => x.Put(It.IsAny<Uri>(), "application/json", jsonDocTypeAdded)).Returns(fakeHttpResponse.Object);
		 	fakeServer.Setup(x => x.Address).Returns(new Uri(url));
		 	fakeDatabaseInfo.Setup(x => x.Name).Returns(databaseName);
		 	fakeHttpResponse.Setup(x => x.StatusCode).Returns(HttpStatusCode.Created);
			fakeHttpResponse.Setup(x => x.Body).Returns("{\"ok\":true,\"id\":\"fe875b98-0ef2-42c2-9c7f-94ab94432250\",\"rev\":\"1-0eb046deef235498747e44e63846b739\"}");
			fakeSerializer.Setup(x => x.Deserialize<Document>(fakeHttpResponse.Object.Body)).Returns(new Document(new Guid("fe875b98-0ef2-42c2-9c7f-94ab94432250"), "1-0eb046deef235498747e44e63846b739"));
			
			IDatabase database = new Database(fakeServer.Object, fakeDatabaseInfo.Object, fakeRestClient.Object, fakeSerializer.Object);
		 	database.SaveDocument<Manager>(manager);
		 	
		 	fakeSerializer.Verify(x => x.Serialize(manager));
		 	fakeSerializer.VerifyGet(x => x.ContentType);
		 	fakeRestClient.Verify(x => x.Put(It.IsAny<Uri>(), "application/json", jsonDocTypeAdded));
		 	fakeHttpResponse.Verify(x => x.StatusCode);
		 	fakeSerializer.Verify(x => x.Deserialize<Document>(fakeHttpResponse.Object.Body));
		 	
		 	Assert.AreNotEqual(manager.Id, default(Guid));
		 	Assert.AreEqual(fakeHttpResponse.Object.StatusCode, HttpStatusCode.Created);
		 }
	}
	
	[TestFixture]
	[Category("Unit")]
	public class When_retrieving_a_document_by_id : OttomanSpecBase<Database>
	{
		private string Url { get; set; }
		private string DatabaseName { get; set; }
		private Guid Id { get; set; }
		private Uri RequestUrl { get; set; }
		private string Body { get; set; }
		private string Json { get; set; }
		private Mock<IServer> FakeServer { get; set; }
		private Mock<IDatabaseInfo> FakeDatabaseInfo { get; set; }
		private Mock<IRestClient> FakeRestClient { get; set; }
		private Mock<ISerializer> FakeSerializer { get; set; }
		
		protected override Database EstablishContext()
		{
			Url = "http://127.0.0.1:5984/";
			DatabaseName = "test";
			Id = new Guid("fe875b98-0ef2-42c2-9c7f-94ab94432250");
			RequestUrl = new Uri(Url + DatabaseName + "/" + Id);
			Body = "{\"Subordinates\":[{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":40.0,\"Id\":\"4c5b075c-b87e-46b9-9108-6dd3a647953b\",\"Name\":\"Bob\",\"Login\":\"bbob\"},{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":40.0,\"Id\":\"b14818db-c975-4109-a94a-452632ee161b\",\"Name\":\"Alice\",\"Login\":\"aalice\"},{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":20.0,\"Id\":\"8f0a0036-319f-49e6-83e7-f84971f9aa5c\",\"Name\":\"Eve\",\"Login\":\"eeve\"}],\"Name\":\"Chris\",\"Login\":\"cchandler\",\"doc_type\":\"DocType\",\"_rev\":\"1-6854b67b702ecc50919aedea24a66499\"}";
			string jsonMinusDocType = "{\"Subordinates\":[{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":40.0,\"Id\":\"4c5b075c-b87e-46b9-9108-6dd3a647953b\",\"Name\":\"Bob\",\"Login\":\"bbob\"},{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":40.0,\"Id\":\"b14818db-c975-4109-a94a-452632ee161b\",\"Name\":\"Alice\",\"Login\":\"aalice\"},{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":20.0,\"Id\":\"8f0a0036-319f-49e6-83e7-f84971f9aa5c\",\"Name\":\"Eve\",\"Login\":\"eeve\"}],\"Name\":\"Chris\",\"Login\":\"cchandler\"}";
			string jsonMinusRev = "{\"Subordinates\":[{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":40.0,\"Id\":\"4c5b075c-b87e-46b9-9108-6dd3a647953b\",\"Name\":\"Bob\",\"Login\":\"bbob\"},{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":40.0,\"Id\":\"b14818db-c975-4109-a94a-452632ee161b\",\"Name\":\"Alice\",\"Login\":\"aalice\"},{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":20.0,\"Id\":\"8f0a0036-319f-49e6-83e7-f84971f9aa5c\",\"Name\":\"Eve\",\"Login\":\"eeve\"}],\"Name\":\"Chris\",\"Login\":\"cchandler\",\"_id\":\"fe875b98-0ef2-42c2-9c7f-94ab94432250\"}";
			string jsonMinusId = "{\"Subordinates\":[{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":40.0,\"Id\":\"4c5b075c-b87e-46b9-9108-6dd3a647953b\",\"Name\":\"Bob\",\"Login\":\"bbob\"},{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":40.0,\"Id\":\"b14818db-c975-4109-a94a-452632ee161b\",\"Name\":\"Alice\",\"Login\":\"aalice\"},{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":20.0,\"Id\":\"8f0a0036-319f-49e6-83e7-f84971f9aa5c\",\"Name\":\"Eve\",\"Login\":\"eeve\"}],\"Name\":\"Chris\",\"Login\":\"cchandler\"}";
			Json = "{\"Subordinates\":[{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":40.0,\"Id\":\"4c5b075c-b87e-46b9-9108-6dd3a647953b\",\"Name\":\"Bob\",\"Login\":\"bbob\"},{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":40.0,\"Id\":\"b14818db-c975-4109-a94a-452632ee161b\",\"Name\":\"Alice\",\"Login\":\"aalice\"},{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":20.0,\"Id\":\"8f0a0036-319f-49e6-83e7-f84971f9aa5c\",\"Name\":\"Eve\",\"Login\":\"eeve\"}],\"Name\":\"Chris\",\"Login\":\"cchandler\",\"Id\":\"fe875b98-0ef2-42c2-9c7f-94ab94432250\"}";
			
			FakeServer = new Mock<IServer>();
			FakeServer.SetupGet(x => x.Address).Returns(new Uri(Url));
			
			FakeDatabaseInfo = new Mock<IDatabaseInfo>();
			FakeDatabaseInfo.SetupGet(x => x.Name).Returns(DatabaseName);
			
			FakeRestClient = new Mock<IRestClient>();
			FakeRestClient.Setup(x => x.Get(RequestUrl)).Returns(new HttpResponse(HttpStatusCode.OK, Body));
			
			FakeSerializer = new Mock<ISerializer>();
			FakeSerializer.Setup(x => x.RemoveKeyFrom(Body, "doc_type")).Returns(jsonMinusDocType);
			FakeSerializer.Setup(x => x.RemoveKeyFrom(jsonMinusDocType, "_rev")).Returns(jsonMinusRev);
			FakeSerializer.Setup(x => x.RemoveKeyFrom(jsonMinusRev, "_id")).Returns(jsonMinusId);
			FakeSerializer.Setup(x => x.AddKeyTo(jsonMinusId, "Id", Id.ToString())).Returns(Json);
			FakeSerializer.Setup(x => x.Deserialize<Manager>(Json));
			
			return new Database(FakeServer.Object, FakeDatabaseInfo.Object, FakeRestClient.Object, FakeSerializer.Object);
		}
		
		[Test]
		public void Should_call_get_and_pass_database_name_and_id_of_document_on_the_url()
		{
			Sut.GetDocument<Manager>(Id.ToString());
			
			FakeRestClient.Verify(x => x.Get(RequestUrl));
		}
		
		[Test]
		public void Should_call_deserialize_with_massaged_json()
		{
			Sut.GetDocument<Manager>(Id.ToString());
			
			FakeSerializer.Verify(x => x.Deserialize<Manager>(Json));
		}
	}

	[TestFixture]
	[Category("Unit")]
	public class When_massaging_json_for_deserialization : OttomanSpecBase<Database>
	{
		private Guid Id { get; set; }
		private Mock<IServer> FakeServer { get; set; }
		private Mock<IDatabaseInfo> FakeDatabaseInfo { get; set; }
		private Mock<IRestClient> FakeRestClient { get; set; }
		private Mock<ISerializer> FakeSerializer { get; set; }

		protected override Database EstablishContext()
		{
			Id = Guid.NewGuid();
			FakeServer = new Mock<IServer>();
			FakeDatabaseInfo = new Mock<IDatabaseInfo>();
			FakeRestClient = new Mock<IRestClient>();
			FakeSerializer = new Mock<ISerializer>();

			return new Database(FakeServer.Object, FakeDatabaseInfo.Object, FakeRestClient.Object, FakeSerializer.Object);
		}

		public void Should_prep_json_by_removing_doc_type_and_adding_id()
		{
			string json = "{\"Subordinates\":[{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":40.0,\"Id\":\"4c5b075c-b87e-46b9-9108-6dd3a647953b\",\"Name\":\"Bob\",\"Login\":\"bbob\"},{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":40.0,\"Id\":\"b14818db-c975-4109-a94a-452632ee161b\",\"Name\":\"Alice\",\"Login\":\"aalice\"},{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":20.0,\"Id\":\"8f0a0036-319f-49e6-83e7-f84971f9aa5c\",\"Name\":\"Eve\",\"Login\":\"eeve\"}],\"Name\":\"Chris\",\"Login\":\"cchandler\",\"doc_type\":\"DocType\",\"_rev\":\"1-6854b67b702ecc50919aedea24a66499\",\"_id\":\"fe875b98-0ef2-42c2-9c7f-94ab94432250\"}";
			string jsonMinusDocType = "{\"Subordinates\":[{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":40.0,\"Id\":\"4c5b075c-b87e-46b9-9108-6dd3a647953b\",\"Name\":\"Bob\",\"Login\":\"bbob\"},{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":40.0,\"Id\":\"b14818db-c975-4109-a94a-452632ee161b\",\"Name\":\"Alice\",\"Login\":\"aalice\"},{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":20.0,\"Id\":\"8f0a0036-319f-49e6-83e7-f84971f9aa5c\",\"Name\":\"Eve\",\"Login\":\"eeve\"}],\"Name\":\"Chris\",\"Login\":\"cchandler\",\"_rev\":\"1-6854b67b702ecc50919aedea24a66499\",\"_id\":\"fe875b98-0ef2-42c2-9c7f-94ab94432250\"}";
			string jsonMinusRev = "{\"Subordinates\":[{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":40.0,\"Id\":\"4c5b075c-b87e-46b9-9108-6dd3a647953b\",\"Name\":\"Bob\",\"Login\":\"bbob\"},{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":40.0,\"Id\":\"b14818db-c975-4109-a94a-452632ee161b\",\"Name\":\"Alice\",\"Login\":\"aalice\"},{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":20.0,\"Id\":\"8f0a0036-319f-49e6-83e7-f84971f9aa5c\",\"Name\":\"Eve\",\"Login\":\"eeve\"}],\"Name\":\"Chris\",\"Login\":\"cchandler\",\"_id\":\"fe875b98-0ef2-42c2-9c7f-94ab94432250\"}";
			string jsonMinusId = "{\"Subordinates\":[{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":40.0,\"Id\":\"4c5b075c-b87e-46b9-9108-6dd3a647953b\",\"Name\":\"Bob\",\"Login\":\"bbob\"},{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":40.0,\"Id\":\"b14818db-c975-4109-a94a-452632ee161b\",\"Name\":\"Alice\",\"Login\":\"aalice\"},{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":20.0,\"Id\":\"8f0a0036-319f-49e6-83e7-f84971f9aa5c\",\"Name\":\"Eve\",\"Login\":\"eeve\"}],\"Name\":\"Chris\",\"Login\":\"cchandler\"}";
			string jsonMassaged = "{\"Subordinates\":[{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":40.0,\"Id\":\"4c5b075c-b87e-46b9-9108-6dd3a647953b\",\"Name\":\"Bob\",\"Login\":\"bbob\"},{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":40.0,\"Id\":\"b14818db-c975-4109-a94a-452632ee161b\",\"Name\":\"Alice\",\"Login\":\"aalice\"},{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":20.0,\"Id\":\"8f0a0036-319f-49e6-83e7-f84971f9aa5c\",\"Name\":\"Eve\",\"Login\":\"eeve\"}],\"Name\":\"Chris\",\"Login\":\"cchandler\",\"Id\":\"fe875b98-0ef2-42c2-9c7f-94ab94432250\"}";

			FakeSerializer.Setup(x => x.RemoveKeyFrom(json, "doc_type"));
			FakeSerializer.Setup(x => x.RemoveKeyFrom(jsonMinusDocType, "_rev"));
			FakeSerializer.Setup(x => x.RemoveKeyFrom(jsonMinusRev, "_id"));
			FakeSerializer.Setup(x => x.AddKeyTo(jsonMinusId, "Id", Id.ToString()));

			string result = Sut.MassageJsonForDeserialization(json, Id.ToString());

			FakeSerializer.Verify(x => x.RemoveKeyFrom(json, "doc_type"));
			FakeSerializer.Verify(x => x.AddKeyTo(jsonMinusDocType, "Id", Id.ToString()));
			Assert.AreEqual(jsonMassaged, result);
		}
	}
}