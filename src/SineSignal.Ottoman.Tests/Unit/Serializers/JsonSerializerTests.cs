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
		public void Should_be_able_to_deserialize_a_new_CouchDatabase_instance_from_json()
		{
			string json = "{\"db_name\":\"test\",\"doc_count\":0,\"doc_del_count\":0,\"update_seq\":0,\"purge_seq\":0,\"compact_running\":false,\"disk_size\":79,\"instance_start_time\":\"1250175373642458\",\"disk_format_version\":4}";

			ISerializer jsonSerializer = new JsonSerializer();
			ICouchDatabase database = jsonSerializer.Deserialize<CouchDatabase>(json);

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
	}
}