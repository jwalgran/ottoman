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
		public void Should_be_able_to_deserialize_a_new_CouchDatabase_instance_from_json()
		{
			string json = "{\"db_name\":\"test\",\"doc_count\":0,\"doc_del_count\":0,\"update_seq\":0,\"purge_seq\":0,\"compact_running\":false,\"disk_size\":79,\"instance_start_time\":\"1250175373642458\",\"disk_format_version\":4}";

			ISerializer jsonSerializer = new JsonSerializer();
			IDatabase database = jsonSerializer.Deserialize<Database>(json);

			Assert.IsNotNull(database);
			Assert.AreEqual("test", database.Name);
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
		public void Should_be_able_to_serialize_and_deserialize_objects()
		{
			var bobOriginal = new Worker {Id = 1, Name = "Bob", Login = "bbob", Hours = 40};
			var aliceOriginal = new Worker { Id = 3, Name = "Alice", Login = "aalice", Hours = 40 };
			var eveOriginal = new Worker { Id = 4, Name = "Eve", Login = "eeve", Hours = 20 };
			var chrisOriginal = new Manager { Id = 2, Name = "Chris", Login = "cchandler", Subordinates = new List<Worker> { bobOriginal, aliceOriginal, eveOriginal } };

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
	}

	public class Employee
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Login { get; set; }

		public override bool Equals(object obj)
		{
			Employee compareTo = obj as Employee;

			if (compareTo != null)
			{
				if (Id == compareTo.Id && Name == compareTo.Name && Login == compareTo.Login)
				{
					return true;
				}
			}

			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}
	}

	public class Worker : Employee
	{
		public double Hours { get; set; }
	}

	public class Manager : Employee
	{
		public IList<Worker> Subordinates { get; set; }
	}
}