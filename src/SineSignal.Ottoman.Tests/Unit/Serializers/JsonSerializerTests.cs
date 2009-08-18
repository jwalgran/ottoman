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
using System.Collections.Generic;
using MbUnit.Framework;

using SineSignal.Ottoman.Serializers;
using SineSignal.Ottoman.Tests.SampleDomain;

namespace SineSignal.Ottoman.Tests.Unit.Serializers
{
	[TestFixture]
	public class JsonSerializerTests
	{
		[Test]
		public void Should_be_able_to_deserialize_a_new_CouchError_instance_from_json()
		{
			string json = "{\"error\":\"file_exists\",\"reason\":\"The database could not be created, the file already exists.\"}";

			ISerializer jsonSerializer = new JsonSerializer();
			ICouchError couchError = jsonSerializer.Deserialize<CouchError>(json);

			Assert.IsNotNull(couchError);
			Assert.AreEqual("file_exists", couchError.Error);
			Assert.AreEqual("The database could not be created, the file already exists.", couchError.Reason);
		}
		
		[Test]
		public void Should_be_able_to_deserialize_a_new_ServerInfo_instance_from_json()
		{
			string json = "{\"couchdb\":\"Welcome\",\"version\":\"0.10.0a800465\"}";

			ISerializer jsonSerializer = new JsonSerializer();
			IServerInfo serverInfo = jsonSerializer.Deserialize<ServerInfo>(json);

			Assert.AreEqual("Welcome", serverInfo.Message);
			Assert.AreEqual("0.10.0a800465", serverInfo.Version);
		}

		[Test]
		public void Should_be_able_to_deserialize_a_new_DatabaseInfo_instance_from_json()
		{
			string json = "{\"db_name\":\"test\",\"doc_count\":0,\"doc_del_count\":0,\"update_seq\":0,\"purge_seq\":0,\"compact_running\":false,\"disk_size\":79,\"instance_start_time\":\"1250175373642458\",\"disk_format_version\":4}";

			ISerializer jsonSerializer = new JsonSerializer();
			IDatabaseInfo databaseInfo = jsonSerializer.Deserialize<DatabaseInfo>(json);

			Assert.IsNotNull(databaseInfo);
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
		public void Should_be_able_to_serialize_and_deserialize_objects()
		{
			var bobOriginal = new Worker(Guid.NewGuid(), "Bob", "bbob", new Address { Street = "123 Somewhere St.", City = "Kalamazoo", State = "MI", Zip = "12345" }, 40);
			var aliceOriginal = new Worker(Guid.NewGuid(), "Alice", "aalice", new Address { Street = "123 Somewhere St.", City = "Kalamazoo", State = "MI", Zip = "12345" }, 40);
			var eveOriginal = new Worker(Guid.NewGuid(), "Eve", "eeve", new Address { Street = "123 Somewhere St.", City = "Kalamazoo", State = "MI", Zip = "12345" }, 20);
			var chrisOriginal = new Manager(default(Guid), "Chris", "cchandler", new List<Worker> { bobOriginal, aliceOriginal, eveOriginal });

			ISerializer serializer = new JsonSerializer();
			string bobJson = serializer.Serialize<Worker>(bobOriginal);
			string aliceJson = serializer.Serialize<Worker>(aliceOriginal);
			string eveJson = serializer.Serialize<Worker>(eveOriginal);
			string chrisJson = serializer.Serialize<Manager>(chrisOriginal);

			var bobDeserialized = serializer.Deserialize<Worker>(bobJson);
			var aliceDeserialized = serializer.Deserialize<Worker>(aliceJson);
			var eveDeserialized = serializer.Deserialize<Worker>(eveJson);
			var chrisDeserialized = serializer.Deserialize<Manager>(chrisJson);

			Assert.AreEqual(bobOriginal, bobDeserialized);
			Assert.AreEqual(aliceOriginal, aliceDeserialized);
			Assert.AreEqual(eveOriginal, eveDeserialized);
			Assert.AreEqual(chrisOriginal, chrisDeserialized);
		}

		[Test]
		public void Should_be_able_to_deserialize_a_new_Document_instance_from_json()
		{
			string json = "{\"ok\":true,\"id\":\"fe875b98-0ef2-42c2-9c7f-94ab94432250\",\"rev\":\"1-0eb046deef235498747e44e63846b739\"}";

			ISerializer jsonSerializer = new JsonSerializer();
			IDocument couchDocument = jsonSerializer.Deserialize<Document>(json);

			Assert.IsNotNull(couchDocument);
			Assert.AreEqual(new Guid("fe875b98-0ef2-42c2-9c7f-94ab94432250"), couchDocument.Id);
			Assert.AreEqual("1-0eb046deef235498747e44e63846b739", couchDocument.Revision);
		}
	}
}