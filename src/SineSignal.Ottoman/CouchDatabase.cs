#region License

// <copyright file="CouchDatabase.cs" company="SineSignal, LLC.">
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

using Newtonsoft.Json;

namespace SineSignal.Ottoman
{
	/// <summary>
	/// Models the response from CouchDB when retrieving a database.
	/// </summary>
	/// <remarks>
	///	An example response from CouchDB:
	/// "{\"db_name\":\"test\",\"doc_count\":0,\"doc_del_count\":0,\"update_seq\":0,\"purge_seq\":0,\"compact_running\":false,\"disk_size\":79,\"instance_start_time\":\"1250175373642458\",\"disk_format_version\":4}";
	/// </remarks>
	public class CouchDatabase : ICouchDatabase
	{
		/// <summary>
		/// Gets or sets the name of the database.
		/// </summary>
		/// <value>The database name.</value>
		[JsonProperty("db_name")]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the number of documents in the database.
		/// </summary>
		/// <value>The doc count.</value>
		[JsonProperty("doc_count")]
		public int DocCount { get; set; }

		/// <summary>
		/// Gets or sets the number of documents that have been deleted in the database.
		/// </summary>
		/// <value>The doc delete count.</value>
		[JsonProperty("doc_del_count")]
		public int DocDelCount { get; set; }

		/// <summary>
		/// Gets or sets the update sequence.
		/// </summary>
		/// <value>The update sequence.</value>
		[JsonProperty("update_seq")]
		public int UpdateSequence { get; set; }

		/// <summary>
		/// Gets or sets the purge sequence.
		/// </summary>
		/// <value>The purge sequence.</value>
		[JsonProperty("purge_seq")]
		public int PurgeSequence { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether database compact is running.
		/// </summary>
		/// <value><c>true</c> if [compact running]; otherwise, <c>false</c>.</value>
		[JsonProperty("compact_running")]
		public bool CompactRunning { get; set; }

		/// <summary>
		/// Gets or sets the size of the disk space being used by the database.
		/// </summary>
		/// <value>The size being used on the disk.</value>
		[JsonProperty("disk_size")]
		public double DiskSize { get; set; }

		/// <summary>
		/// Gets or sets the instance start time.
		/// </summary>
		/// <value>The instance start time.</value>
		[JsonProperty("instance_start_time")]
		public string InstanceStartTime { get; set; }

		/// <summary>
		/// Gets or sets the disk format version.
		/// </summary>
		/// <value>The disk format version.</value>
		[JsonProperty("disk_format_version")]
		public int DiskFormatVersion { get; set; }

		public CouchDatabase(string name, int docCount, int docDelCount, int updateSequence, int purgeSequence, bool compactRunning, double diskSize, string instanceStartTime, int diskFormatVersion)
		{
			Name = name;
			DocCount = docCount;
			DocDelCount = docDelCount;
			UpdateSequence = updateSequence;
			PurgeSequence = purgeSequence;
			CompactRunning = compactRunning;
			DiskSize = diskSize;
			InstanceStartTime = instanceStartTime;
			DiskFormatVersion = diskFormatVersion;
		}
	}
}