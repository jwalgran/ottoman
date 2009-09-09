#region License

// <copyright file="ServerTests.cs" company="SineSignal, LLC.">
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

using SineSignal.Ottoman.Exceptions;
using SineSignal.Ottoman.Model;

namespace SineSignal.Ottoman.Tests.Integration
{
	[TestFixture]
	[Category("Integration")]
	public class When_we_are_really_creating_a_database : OttomanSpecBase<IServer>
	{
		protected override IServer EstablishContext()
		{
			return Server.Connect(Constants.DefaultAddress);
		}

		protected override void Because()
		{
			Sut.CreateDatabase(Constants.DatabaseName);
		}

		protected override void AfterEachSpecification()
		{
			Sut.DeleteDatabase(Constants.DatabaseName);
		}

		[Test]
		public void Should_create_database()
		{
			IDatabase database = Sut.GetDatabase(Constants.DatabaseName);

			Assert.AreEqual(Constants.DatabaseName, database.Info.Name);
		}
	}

	[TestFixture]
	[Category("Integration")]
	public class When_we_are_really_deleting_a_database : OttomanSpecBase<IServer>
	{
		protected override IServer EstablishContext()
		{
			return Server.Connect(Constants.DefaultAddress);
		}

		protected override void Because()
		{
			Sut.CreateDatabase(Constants.DatabaseName);
			Sut.DeleteDatabase(Constants.DatabaseName);
		}

		[Test]
		[ExpectedException(typeof(CannotGetDatabaseException))]
		public void Should_delete_database()
		{
			IDatabase database = Sut.GetDatabase(Constants.DatabaseName);
		}
	}

	[TestFixture]
	[Category("Integration")]
	public class When_we_are_really_retrieving_a_database : OttomanSpecBase<IServer>
	{
		private IDatabase Database { get; set; }

		protected override IServer EstablishContext()
		{
			return Server.Connect(Constants.DefaultAddress);
		}

		protected override void Because()
		{
			Sut.CreateDatabase(Constants.DatabaseName);
			Database = Sut.GetDatabase(Constants.DatabaseName);
		}

		protected override void AfterEachSpecification()
		{
			Sut.DeleteDatabase(Constants.DatabaseName);
		}

		[Test]
		public void Should_get_database()
		{	
			Assert.IsNotNull(Database);
			Assert.AreEqual(Constants.DatabaseName, Database.Info.Name);
		}
	}
	
	[TestFixture]
	[Category("Integration")]
	public class When_we_are_really_retrieving_a_list_of_databases : OttomanSpecBase<IServer>
	{
		private string[] Data { get; set; }
		private string[] RetrievedDatabaseNames { get; set; }

		protected override IServer EstablishContext()
		{
			Data = new string[5];
			
			for (int index = 0; index < Data.Length; index++)
			{
				Data[index] = Constants.DatabaseName + index;
			}

			return Server.Connect(Constants.DefaultAddress);
		}

		protected override void Because()
		{
			for (int index = 0; index < Data.Length; index++)
			{
				Sut.CreateDatabase(Data[index]);
			}

			RetrievedDatabaseNames = Sut.GetDatabases();
		}

		protected override void AfterEachSpecification()
		{
			for (int index = 0; index < Data.Length; index++)
			{
				Sut.DeleteDatabase(Data[index]);
			}
		}

		[Test]
		public void Should_get_database_names()
		{
			Assert.IsNotNull(RetrievedDatabaseNames);
			Assert.AreEqual(Data.Length, RetrievedDatabaseNames.Length);
			
			Array.Sort(RetrievedDatabaseNames);
			
			for (int index = 0; index < RetrievedDatabaseNames.Length; index++)
			{
				Assert.AreEqual(Data[index], RetrievedDatabaseNames[index]);
			}
		}
	}

	[TestFixture]
	[Category("Integration")]
	public class When_we_are_really_retrieving_info_about_the_server : OttomanSpecBase<IServer>
	{
		private IServerInfo ServerInfo { get; set; }

		protected override IServer EstablishContext()
		{
			return Server.Connect(Constants.DefaultAddress);
		}

		protected override void Because()
		{
			ServerInfo = Sut.GetInfo();
		}

		[Test]
		public void Should_get_info()
		{
			Assert.IsNotNull(ServerInfo);
			Assert.AreEqual(Constants.CouchDBMessage, ServerInfo.Message);
			Assert.AreEqual(Constants.CouchDBVersion, ServerInfo.Version);
		}
	}

	[TestFixture]
	[Category("Integration")]
	public class When_we_are_really_retrieving_uuids_from_the_server : OttomanSpecBase<IServer>
	{
		private Guid[] Uuids { get; set; }

		protected override IServer EstablishContext()
		{
			return Server.Connect(Constants.DefaultAddress);
		}

		protected override void Because()
		{
			Uuids = Sut.GetUuids(5);
		}

		[Test]
		public void Should_get_uuids()
		{
			Assert.IsNotNull(Uuids);
			Assert.AreEqual(5, Uuids.Length);
			
			for (int index = 0; index < Uuids.Length; index++)
			{
				if (Uuids[index] == Guid.Empty)
				{
					Assert.Fail("One or more of the Uuids is empty.");
					break;
				}
			}
		}
	}
}