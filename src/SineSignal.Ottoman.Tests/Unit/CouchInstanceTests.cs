#region License

// <copyright file="CouchInstanceTests.cs" company="SineSignal, LLC.">
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

using MbUnit.Framework;
using Moq;

namespace SineSignal.Ottoman.Tests.Unit
{
	[TestFixture]
	public class CouchInstanceTests
	{
		[Test]
		[ExpectedArgumentNullException]
		public void Should_throw_argument_null_exception_when_proxy_is_null_during_construction()
		{
			ICouchInstance couchInstance = new CouchInstance(null);
		}

		[Test]
		public void Should_set_Proxy_when_given_an_instantiated_proxy_during_construction()
		{
			var mockProxy = new Mock<IProxy>();

			ICouchInstance couchInstance = new CouchInstance(mockProxy.Object);

			Assert.IsNotNull(couchInstance.Proxy);
			Assert.AreEqual(mockProxy.Object, couchInstance.Proxy);
		}

		[Test]
		[ExpectedArgumentNullException]
		public void Should_throw_an_argument_null_exception_when_CreateDatabase_is_called_with_a_null_string()
		{
			var mockProxy = new Mock<IProxy>();

			ICouchInstance couchInstance = new CouchInstance(mockProxy.Object);
			couchInstance.CreateDatabase(null);
		}
		
		[Test]
		[ExpectedArgumentNullException]
		public void Should_throw_an_argument_null_exception_when_CreateDatabase_is_called_with_an_empty_string()
		{
			var mockProxy = new Mock<IProxy>();

			ICouchInstance couchInstance = new CouchInstance(mockProxy.Object);
			couchInstance.CreateDatabase("");
		}
		
		[Test]
		public void Should_create_database_when_given_a_valid_name()
		{
			string databaseName = "test";
			string result = "{\"ok\":true}";
			var mockProxy = new Mock<IProxy>();
			mockProxy.Setup(x => x.Put(databaseName)).Returns(result);

			ICouchInstance couchInstance = new CouchInstance(mockProxy.Object);
			couchInstance.CreateDatabase(databaseName);
			
			mockProxy.Verify(x => x.Put(databaseName));
		}
	}
}