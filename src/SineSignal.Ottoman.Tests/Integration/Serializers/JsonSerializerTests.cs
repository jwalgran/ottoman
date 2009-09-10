#region License

// <copyright file="JsonSerializerTests.cs" company="SineSignal, LLC.">
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

using MbUnit.Framework;

using SineSignal.Ottoman.Model;
using SineSignal.Ottoman.Serializers;
using SineSignal.Ottoman.Tests.SampleDomain;

namespace SineSignal.Ottoman.Tests.Integration.Serializers
{
	[TestFixture]
	[Category("Unit")]
	public class When_deserializing_from_json
	{
		public void Should_be_able_to_deserialize_a_CouchError_instance()
		{
			string json = "{\"error\":\"file_exists\",\"reason\":\"The database could not be created, the file already exists.\"}";
			
			ISerializer jsonSerializer = new JsonSerializer();
			IErrorInfo couchError = jsonSerializer.Deserialize<ErrorInfo>(json);
			
			Assert.AreEqual("file_exists", couchError.Error);
			Assert.AreEqual("The database could not be created, the file already exists.", couchError.Reason);
		}

		[Test]
		public void Should_be_able_to_deserialize_a_ServerInfo_instance()
		{
			string json = "{\"couchdb\":\"Welcome\",\"version\":\"0.10.0a800465\"}";
			
			ISerializer jsonSerializer = new JsonSerializer();
			IServerInfo serverInfo = jsonSerializer.Deserialize<ServerInfo>(json);
			
			Assert.AreEqual("Welcome", serverInfo.Message);
			Assert.AreEqual("0.10.0a800465", serverInfo.Version);
		}

		[Test]
		public void Should_be_able_to_deserialize_a_DatabaseInfo_instance()
		{
			string json = "{\"db_name\":\"test\",\"doc_count\":0,\"doc_del_count\":0,\"update_seq\":0,\"purge_seq\":0,\"compact_running\":false,\"disk_size\":79,\"instance_start_time\":\"1250175373642458\",\"disk_format_version\":4}";
			
			ISerializer jsonSerializer = new JsonSerializer();
			IDatabaseInfo databaseInfo = jsonSerializer.Deserialize<DatabaseInfo>(json);
			
			Assert.AreEqual("test", databaseInfo.Name);
			Assert.AreEqual(0, databaseInfo.DocCount);
			Assert.AreEqual(0, databaseInfo.DocDelCount);
			Assert.AreEqual(0, databaseInfo.UpdateSequence);
			Assert.AreEqual(0, databaseInfo.PurgeSequence);
			Assert.AreEqual(false, databaseInfo.CompactRunning);
			Assert.AreEqual(79, databaseInfo.DiskSize);
			Assert.AreEqual("1250175373642458", databaseInfo.InstanceStartTime);
			Assert.AreEqual(4, databaseInfo.DiskFormatVersion);
		}

		[Test]
		public void Should_be_able_to_deserialize_a_Document_instance()
		{
			string json = "{\"ok\":true,\"id\":\"fe875b98-0ef2-42c2-9c7f-94ab94432250\",\"rev\":\"1-0eb046deef235498747e44e63846b739\"}";
			
			ISerializer jsonSerializer = new JsonSerializer();
			IDocument couchDocument = jsonSerializer.Deserialize<Document>(json);
			
			Assert.AreEqual(new Guid("fe875b98-0ef2-42c2-9c7f-94ab94432250"), couchDocument.Id);
			Assert.AreEqual("1-0eb046deef235498747e44e63846b739", couchDocument.Revision);
		}
		
		[Test]
		public void Should_be_able_to_deserialize_a_sample_domain()
		{
			Manager manager = Manager.CreateManager();

			string managerJson = "{\"Subordinates\":[{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":40.0,\"Id\":\"6bcdea2f-2439-4785-ab59-2ee612435705\",\"Name\":\"Bob\",\"Login\":\"bbob\"},{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":40.0,\"Id\":\"b0d156c9-ea3f-4c4f-b49d-ab19bff64dd8\",\"Name\":\"Alice\",\"Login\":\"aalice\"},{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":20.0,\"Id\":\"12b6dbbc-44e8-43c2-8142-11fc6c1d23df\",\"Name\":\"Eve\",\"Login\":\"eeve\"}],\"Id\":\"dfd6ef13-f8d2-4f9a-b265-0d8ecfe717b3\",\"Name\":\"Chris\",\"Login\":\"cchandler\"}";

			ISerializer serializer = new JsonSerializer();
			Manager managerDeserialized = serializer.Deserialize<Manager>(managerJson);
			
			Assert.AreEqual(manager, managerDeserialized);
			Assert.AreEqual(3, managerDeserialized.Subordinates.Count);
		}
	}
	
	[TestFixture]
	[Category("Unit")]
	public class When_serializing_to_json
	{	
		[Test]
		public void Should_be_able_to_serialize_sample_domain()
		{
			Manager manager = Manager.CreateManager();
			string managerJson = "{\"Subordinates\":[{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":40.0,\"Id\":\"6bcdea2f-2439-4785-ab59-2ee612435705\",\"Name\":\"Bob\",\"Login\":\"bbob\"},{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":40.0,\"Id\":\"b0d156c9-ea3f-4c4f-b49d-ab19bff64dd8\",\"Name\":\"Alice\",\"Login\":\"aalice\"},{\"Address\":{\"Street\":\"123 Somewhere St.\",\"City\":\"Kalamazoo\",\"State\":\"MI\",\"Zip\":\"12345\"},\"Hours\":20.0,\"Id\":\"12b6dbbc-44e8-43c2-8142-11fc6c1d23df\",\"Name\":\"Eve\",\"Login\":\"eeve\"}],\"Id\":\"dfd6ef13-f8d2-4f9a-b265-0d8ecfe717b3\",\"Name\":\"Chris\",\"Login\":\"cchandler\"}";
			
			ISerializer serializer = new JsonSerializer();
			string managerSerialized = serializer.Serialize(manager);
			
			Assert.AreEqual(managerJson, managerSerialized);
		}
	}
}