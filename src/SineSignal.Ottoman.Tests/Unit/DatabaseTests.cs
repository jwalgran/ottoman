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
using System.Collections.Generic;
using System.Net;

using MbUnit.Framework;
using Moq;

using SineSignal.Ottoman.Proxy;
using SineSignal.Ottoman.Serializers;
using SineSignal.Ottoman.Tests.SampleDomain;

namespace SineSignal.Ottoman.Tests.Unit
{
	[TestFixture]
	public class DatabaseTests
	{
		[Test]
		[Row("test")]
		public void Should_be_able_to_retrieve_info_about_the_database_and_update_the_info_property(string databaseName)
		{
			string url = "http://127.0.0.1:5984/";
			UriBuilder requestUrl = new UriBuilder(url);
			requestUrl.Path = databaseName;
			string body = "{\"db_name\":\"" + databaseName + "\",\"doc_count\":0,\"doc_del_count\":0,\"update_seq\":0,\"purge_seq\":0,\"compact_running\":false,\"disk_size\":79,\"instance_start_time\":\"1250175373642458\",\"disk_format_version\":4}";

			var mockHttpResponse = new Mock<IHttpResponse>();
			mockHttpResponse.Setup(x => x.StatusCode).Returns(HttpStatusCode.OK);
			mockHttpResponse.Setup(x => x.Body).Returns(body);
			
			var mockRestProxy = new Mock<IRestProxy>();
			mockRestProxy.Setup(x => x.Get(requestUrl.Uri)).Returns(mockHttpResponse.Object);

			var mockSerializer = new Mock<ISerializer>();
			mockSerializer.Setup(x => x.Deserialize<DatabaseInfo>(body)).Returns(new DatabaseInfo(databaseName, 0, 0, 0, 0, false, 79, "1250175373642458", 4));
			
			var mockServer = new Mock<IServer>();
			mockServer.Setup(x => x.Url).Returns(new Uri(url));
			mockServer.Setup(x => x.RestProxy).Returns(mockRestProxy.Object);
			mockServer.Setup(x => x.Serializer).Returns(mockSerializer.Object);
			
			IDatabase database = new Database(mockServer.Object, new DatabaseInfo(databaseName, 0, 0, 0, 0, false, 0, "", 0));
			database.UpdateInfo();

			mockServer.VerifyGet(x => x.Url, Times.AtLeastOnce());
			mockServer.VerifyGet(x => x.RestProxy, Times.AtLeastOnce());
			mockServer.VerifyGet(x => x.Serializer, Times.AtLeastOnce());
			mockRestProxy.Verify(x => x.Get(requestUrl.Uri), Times.AtLeastOnce());
			mockSerializer.Verify(x => x.Deserialize<DatabaseInfo>(body), Times.AtLeastOnce());

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

		 	mockServer.Setup(x => x.Serializer).Returns(mockSerializer.Object);
		 	mockSerializer.Setup(x => x.Serialize(manager)).Returns(json);
		 	mockSerializer.Setup(x => x.Remove(json, "Id")).Returns(jsonMinusId);
		 	mockSerializer.Setup(x => x.Add(jsonMinusId, "doc_type", docType)).Returns(jsonDocTypeAdded);
		 	mockServer.Setup(x => x.RestProxy).Returns(mockRestProxy.Object);
		 	mockSerializer.Setup(x => x.ContentType).Returns("application/json");
			mockRestProxy.Setup(x => x.Put(It.IsAny<Uri>(), "application/json", jsonDocTypeAdded)).Returns(mockHttpResponse.Object);
		 	mockServer.Setup(x => x.Url).Returns(new Uri(url));
		 	mockDatabaseInfo.Setup(x => x.Name).Returns(databaseName);
		 	mockHttpResponse.Setup(x => x.StatusCode).Returns(HttpStatusCode.Created);
			mockHttpResponse.Setup(x => x.Body).Returns("{\"ok\":true,\"id\":\"fe875b98-0ef2-42c2-9c7f-94ab94432250\",\"rev\":\"1-0eb046deef235498747e44e63846b739\"}");
			mockSerializer.Setup(x => x.Deserialize<Document>(mockHttpResponse.Object.Body)).Returns(new Document(new Guid("fe875b98-0ef2-42c2-9c7f-94ab94432250"), "1-0eb046deef235498747e44e63846b739"));
			
			IDatabase database = new Database(mockServer.Object, mockDatabaseInfo.Object);
		 	database.SaveDocument<Manager>(manager);
		 	
		 	mockServer.VerifyGet(x => x.Serializer);
		 	mockSerializer.Verify(x => x.Serialize(manager));
		 	mockSerializer.Verify(x => x.Remove(json, "Id"));
		 	mockSerializer.Verify(x => x.Add(jsonMinusId, "doc_type", docType));
		 	mockServer.VerifyGet(x => x.RestProxy);
		 	mockSerializer.VerifyGet(x => x.ContentType);
		 	mockRestProxy.Verify(x => x.Put(It.IsAny<Uri>(), "application/json", jsonDocTypeAdded));
		 	mockHttpResponse.Verify(x => x.StatusCode);
		 	mockSerializer.Verify(x => x.Deserialize<Document>(mockHttpResponse.Object.Body));
		 	
		 	Assert.AreNotEqual(manager.Id, default(Guid));
		 	Assert.AreEqual(mockHttpResponse.Object.StatusCode, HttpStatusCode.Created);
		 }
	}
}