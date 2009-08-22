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

using SineSignal.Ottoman.Proxy;
using SineSignal.Ottoman.Serializers;
using SineSignal.Ottoman.Tests.SampleDomain;

namespace SineSignal.Ottoman.Tests.Unit
{
	[TestFixture]
	public class When_updating_the_database_info : OttomanSpecBase<Database>
	{
		private string Url { get; set; }
		private string DatabaseName { get; set; }
		private Uri RequestUrl { get; set; }
		private Mock<IServer> MockServer { get; set; }
		private Mock<IDatabaseInfo> MockDatabaseInfo { get; set; }
		private string Body { get; set; }
		private Mock<IRestProxy> MockRestProxy { get; set; }
		private Mock<ISerializer> MockSerializer { get; set; }
		
		protected override Database EstablishContext()
		{
			// Arrange
			Url = "http://127.0.0.1:5984/";
			DatabaseName = "test";
			RequestUrl = new Uri(Url + DatabaseName);
			
			MockServer = new Mock<IServer>();
			MockServer.SetupGet(x => x.Url).Returns(new Uri(Url));

			MockDatabaseInfo = new Mock<IDatabaseInfo>();
			MockDatabaseInfo.SetupGet(x => x.Name).Returns(DatabaseName);
			
			Body = "{\"db_name\":\"" + DatabaseName + "\",\"doc_count\":0,\"doc_del_count\":0,\"update_seq\":0,\"purge_seq\":0,\"compact_running\":false,\"disk_size\":79,\"instance_start_time\":\"1250175373642458\",\"disk_format_version\":4}";
			MockRestProxy = new Mock<IRestProxy>();
			MockRestProxy.Setup(x => x.Get(RequestUrl)).Returns(new HttpResponse(HttpStatusCode.OK, Body));

			MockSerializer = new Mock<ISerializer>();
			MockSerializer.Setup(x => x.Deserialize<DatabaseInfo>(Body)).Returns(new DatabaseInfo(DatabaseName, 0, 0, 0, 0, false, 79, "1250175373642458", 4));
			
			return new Database(MockServer.Object, MockDatabaseInfo.Object, MockRestProxy.Object, MockSerializer.Object);
		}
		
		[Test]
		public void Should_leverage_Server_to_retrieve_base_url()
		{
			// Act
			Sut.UpdateInfo();
			
			// Assert
			MockServer.VerifyGet(x => x.Url);
		}

		[Test]
		public void Should_leverage_DatabaseInfo_to_retrieve_database_name()
		{
			// Act
			Sut.UpdateInfo();

			// Assert
			MockDatabaseInfo.VerifyGet(x => x.Name);
		}
		
		[Test]
		[Row("test")]
		public void Should_call_get_on_RestProxy_passing_the_database_name_on_the_url()
		{
			// Act
			Sut.UpdateInfo();
			
			// Assert
			MockRestProxy.Verify(x => x.Get(RequestUrl));
		}
		
		[Test]
		public void Should_call_deserialize_on_Serializer_passing_the_response_body()
		{
			// Act
			Sut.UpdateInfo();
			
			// Assert
			MockSerializer.Verify(x => x.Deserialize<DatabaseInfo>(Body));
		}
		
		[Test]
		public void Should_set_DatabaseInfo_from_deserialized_body()
		{
			// Act
			Sut.UpdateInfo();
			
			// Assert
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
	public class When_massaging_json_for_sending : OttomanSpecBase<Database>
	{
		private Mock<IServer> MockServer { get; set; }
		private Mock<IDatabaseInfo> MockDatabaseInfo { get; set; }
		private Mock<IRestProxy> MockRestProxy { get; set; }
		private Mock<ISerializer> MockSerializer { get; set; }
		
		protected override Database EstablishContext()
		{
			MockServer = new Mock<IServer>();
			MockDatabaseInfo = new Mock<IDatabaseInfo>();
			MockRestProxy = new Mock<IRestProxy>();
			MockSerializer = new Mock<ISerializer>();
			
			return new Database(MockServer.Object, MockDatabaseInfo.Object, MockRestProxy.Object, MockSerializer.Object);
		}

		public void Should_prep_json_by_removing_id_and_adding_doc_type()
		{
			// Arrange
			string json = "{\"Id\":\"fe875b98-0ef2-42c2-9c7f-94ab94432250\",\"Subordinates\":[{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":40.0,\"Id\":\"4c5b075c-b87e-46b9-9108-6dd3a647953b\",\"Name\":\"Bob\",\"Login\":\"bbob\"},{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":40.0,\"Id\":\"b14818db-c975-4109-a94a-452632ee161b\",\"Name\":\"Alice\",\"Login\":\"aalice\"},{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":20.0,\"Id\":\"8f0a0036-319f-49e6-83e7-f84971f9aa5c\",\"Name\":\"Eve\",\"Login\":\"eeve\"}],\"Name\":\"Chris\",\"Login\":\"cchandler\"}";
			string jsonMinusId = "{\"Subordinates\":[{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":40.0,\"Id\":\"4c5b075c-b87e-46b9-9108-6dd3a647953b\",\"Name\":\"Bob\",\"Login\":\"bbob\"},{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":40.0,\"Id\":\"b14818db-c975-4109-a94a-452632ee161b\",\"Name\":\"Alice\",\"Login\":\"aalice\"},{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":20.0,\"Id\":\"8f0a0036-319f-49e6-83e7-f84971f9aa5c\",\"Name\":\"Eve\",\"Login\":\"eeve\"}],\"Name\":\"Chris\",\"Login\":\"cchandler\"}";
			string jsonMassaged = "{\"Subordinates\":[{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":40.0,\"Id\":\"4c5b075c-b87e-46b9-9108-6dd3a647953b\",\"Name\":\"Bob\",\"Login\":\"bbob\"},{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":40.0,\"Id\":\"b14818db-c975-4109-a94a-452632ee161b\",\"Name\":\"Alice\",\"Login\":\"aalice\"},{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":20.0,\"Id\":\"8f0a0036-319f-49e6-83e7-f84971f9aa5c\",\"Name\":\"Eve\",\"Login\":\"eeve\"}],\"Name\":\"Chris\",\"Login\":\"cchandler\",\"doc_type\":\"DocType\"}";

			MockSerializer.Setup(x => x.RemoveKeyFrom(json, "Id"));
			MockSerializer.Setup(x => x.AddKeyTo(jsonMinusId, "doc_type", "DocType"));
			
			// Act
			string result = Sut.MassageJsonForSending(json, "DocType");
			
			// Assert
			MockSerializer.Verify(x => x.RemoveKeyFrom(json, "Id"));
			MockSerializer.Verify(x => x.AddKeyTo(jsonMinusId, "doc_type", "DocType"));
			Assert.AreEqual(jsonMassaged, result);
		}
	}

	[TestFixture]
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
		 	
		 	var mockServer = new Mock<IServer>();
		 	var mockDatabaseInfo = new Mock<IDatabaseInfo>();
		 	var mockSerializer = new Mock<ISerializer>();
		 	var mockRestProxy = new Mock<IRestProxy>();
		 	var mockHttpResponse = new Mock<IHttpResponse>();
		 	var mockDocument = new Mock<IDocument>();

		 	mockSerializer.Setup(x => x.Serialize(manager)).Returns(json);
		 	mockSerializer.Setup(x => x.RemoveKeyFrom(json, "Id")).Returns(jsonMinusId);
		 	mockSerializer.Setup(x => x.AddKeyTo(jsonMinusId, "doc_type", docType)).Returns(jsonDocTypeAdded);
		 	mockSerializer.Setup(x => x.ContentType).Returns("application/json");
			mockRestProxy.Setup(x => x.Put(It.IsAny<Uri>(), "application/json", jsonDocTypeAdded)).Returns(mockHttpResponse.Object);
		 	mockServer.Setup(x => x.Url).Returns(new Uri(url));
		 	mockDatabaseInfo.Setup(x => x.Name).Returns(databaseName);
		 	mockHttpResponse.Setup(x => x.StatusCode).Returns(HttpStatusCode.Created);
			mockHttpResponse.Setup(x => x.Body).Returns("{\"ok\":true,\"id\":\"fe875b98-0ef2-42c2-9c7f-94ab94432250\",\"rev\":\"1-0eb046deef235498747e44e63846b739\"}");
			mockSerializer.Setup(x => x.Deserialize<Document>(mockHttpResponse.Object.Body)).Returns(new Document(new Guid("fe875b98-0ef2-42c2-9c7f-94ab94432250"), "1-0eb046deef235498747e44e63846b739"));
			
			IDatabase database = new Database(mockServer.Object, mockDatabaseInfo.Object, mockRestProxy.Object, mockSerializer.Object);
		 	database.SaveDocument<Manager>(manager);
		 	
		 	mockSerializer.Verify(x => x.Serialize(manager));
		 	mockSerializer.VerifyGet(x => x.ContentType);
		 	mockRestProxy.Verify(x => x.Put(It.IsAny<Uri>(), "application/json", jsonDocTypeAdded));
		 	mockHttpResponse.Verify(x => x.StatusCode);
		 	mockSerializer.Verify(x => x.Deserialize<Document>(mockHttpResponse.Object.Body));
		 	
		 	Assert.AreNotEqual(manager.Id, default(Guid));
		 	Assert.AreEqual(mockHttpResponse.Object.StatusCode, HttpStatusCode.Created);
		 }
	}
}