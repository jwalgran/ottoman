#region License

// <copyright file="SeededLongGeneratorTests.cs" company="SineSignal, LLC.">
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
using Moq;

using SineSignal.Ottoman.Generators;

namespace SineSignal.Ottoman.Tests.Unit.Generators
{
    [TestFixture]
	[Category("Unit")]
    public class When_creating_a_SeededLongGenerator_with_default_reseed_interval : OttomanSpecBase<SeededLongGenerator>
    {
		private Mock<IServer> MockServer { get; set; }
		
    	protected override SeededLongGenerator EstablishContext()
    	{
    		// Arrange
			MockServer = new Mock<IServer>();
			return new SeededLongGenerator(MockServer.Object);
    	}

        [Test]
        public void Should_have_a_Server_set_to_passed_in_Server()
        {
			Assert.AreEqual(MockServer.Object, Sut.Server);
        }

        [Test]
        public void Should_have_a_default_reseed_interval_option_set_to_the_max_int_value()
        {
            Assert.AreEqual(Int32.MaxValue, Sut.ReseedInterval);
        }
    }

    [TestFixture]
	[Category("Unit")]
	public class When_generating_an_ID_using_the_SeededLongGenerator_with_default_reseed_interval : OttomanSpecBase<SeededLongGenerator>
    {
		private Mock<IServer> MockServer { get; set; }
		
    	protected override SeededLongGenerator EstablishContext()
    	{
    		// Arrange
			string uuid = "0123456789abcdef0123456789abcdef";
			MockServer = new Mock<IServer>();
    		MockServer.Setup(x => x.GetUuids(1)).Returns(new Guid[] { new Guid(uuid) });
    		
    		return new SeededLongGenerator(MockServer.Object);
    	}

        [Test]
        public void Should_request_a_UUID_from_CouchDB_the_first_time_Generate_is_called()
        {
            // Act
            Sut.Generate();
            Sut.Generate();
            Sut.Generate();

            // Assert
            MockServer.Verify(x => x.GetUuids(1), Times.Once());
        }

        [Test]
        public void Should_return_a_unique_non_seqential_long_integer_value_each_time_Generate_is_called()
        {
            // Act
            var firstID = Sut.Generate();
            var secondID = Sut.Generate();
            var thirdID = Sut.Generate();

            // Assert
			Assert.AreEqual(1408408213, firstID); //these values are known because the uuid 'seed' is fixed by mocking the request
			Assert.AreEqual(2739867258, secondID);
			Assert.AreEqual(3497938919, thirdID);
        }
    }
    
    [TestFixture]
	[Category("Unit")]
    public class When_creating_a_SeededLongGenerator_with_specified_reseed_interval : OttomanSpecBase<SeededLongGenerator>
    {
		private Mock<IServer> MockServer { get; set; }
		
    	protected override SeededLongGenerator EstablishContext()
    	{
			// Arrange
			string uuid = "0123456789abcdef0123456789abcdef";
			MockServer = new Mock<IServer>();
			MockServer.Setup(x => x.GetUuids(1)).Returns(new Guid[] { new Guid(uuid) });

			return new SeededLongGenerator(MockServer.Object, 2);
    	}

		[Test]
		public void Should_have_a_Server_set_to_passed_in_Server()
		{
			Assert.AreEqual(MockServer.Object, Sut.Server);
		}

		[Test]
		public void Should_have_a_reseed_interval_option_set_to_the_passed_in_value()
		{
			Assert.AreEqual(2, Sut.ReseedInterval);
		}
    }

	[TestFixture]
	[Category("Unit")]
	public class When_generating_an_ID_using_the_SeededLongGenerator_with_specified_reseed_interval : OttomanSpecBase<SeededLongGenerator>
	{
		private Mock<IServer> MockServer { get; set; }

		protected override SeededLongGenerator EstablishContext()
		{
			// Arrange
			string uuid = "0123456789abcdef0123456789abcdef";
			MockServer = new Mock<IServer>();
			MockServer.Setup(x => x.GetUuids(1)).Returns(new Guid[] { new Guid(uuid) });

			return new SeededLongGenerator(MockServer.Object, 2);
		}

		[Test]
		public void Should_request_a_uuid_from_CouchDB_when_the_ReseedInterval_is_met()
		{
			// Act
			Sut.Generate(); //This call should trigger the frist uuid request
			Sut.Generate();
			Sut.Generate(); //This third call should trigger the next uuid request since ReseedInterval is 2

			// Assert
			MockServer.Verify(x => x.GetUuids(1), Times.Exactly(2));
		}
	}
}
