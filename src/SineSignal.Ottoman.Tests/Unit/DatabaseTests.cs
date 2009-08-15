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
		 *				POSTS the JSON to the CouchDB server.
		 *				Verifies the POST and returns the object, otherwise throw exception.
		 */
	}
}