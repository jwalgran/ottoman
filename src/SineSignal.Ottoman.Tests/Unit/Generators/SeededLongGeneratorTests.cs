using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using SineSignal.Ottoman;
#region License

// <copyright file="RestProxyTests.cs" company="SineSignal, LLC.">
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
using SineSignal.Ottoman.Generators;
using SineSignal.Ottoman.Proxy;
using MbUnit.Framework;
using Moq;

namespace SineSignal.Ottoman.Tests.Unit.Generators.SeededLongGeneratorTests
{
    [TestFixture]
    public class When_creating_a_SeededLongGenerator
    {
        private SeededLongGenerator _generator;

        [SetUp]
        public void SetUp()
        {
            _generator = new SeededLongGenerator();
        }

        [Test]
        public void Should_have_a_default_ServerURL_option_set_to_localhost()
        {
            Assert.AreEqual(_generator.Options["ServerURL"], "http://127.0.0.1:5984");
        }

        [Test]
        public void Should_have_a_default_RestProxy_option_set_to_a_new_instance_of_RestProxy()
        {
            Assert.IsInstanceOfType<Ottoman.Proxy.RestProxy>(_generator.Options["RestProxy"]);
        }

        [Test]
        public void Should_have_a_default_reseed_interval_option_set_to_the_max_int_value()
        {
            Assert.AreEqual(int.MaxValue,(int)_generator.Options["ReseedInterval"]);
        }
    }

    [TestFixture]
    public class When_generating_an_ID_using_the_SeededLongGenerator
    {
        private Mock<IRestProxy> _mockRestProxy;
        private string _url;
        private Uri _uuidURI;

        [SetUp]
        public void SetUp()
        {
            // Arrange
            _url = "http://127.0.0.1:5984/";
            UriBuilder requestUriBuilder = new UriBuilder(_url);
            string uuid = "0123456789abcdef0123456789abcdef";
            string body = "{\"uuids\":[\"" + uuid + "\"]}";
            requestUriBuilder.Path = "_uuids";
            _uuidURI = requestUriBuilder.Uri;

            var mockHttpResponse = new Mock<IHttpResponse>();
            mockHttpResponse.Setup(x => x.StatusCode).Returns(HttpStatusCode.OK);
            mockHttpResponse.Setup(x => x.Body).Returns(body);

            _mockRestProxy = new Mock<IRestProxy>();
            _mockRestProxy.Setup(x => x.Get(_uuidURI)).Returns(mockHttpResponse.Object);
        }

        [Test]
        public void Should_request_a_UUID_from_CouchDB_the_first_time_Generate_is_called()
        {
            // Act
            var generator = new SeededLongGenerator();
            generator.Options["ServerURL"] = _url;
            generator.Options["RestProxy"] = _mockRestProxy.Object;
            generator.Generate();
            generator.Generate();
            generator.Generate();

            // Assert
            _mockRestProxy.Verify(x => x.Get(_uuidURI), Times.Once());
        }

        [Test]
        public void Should_request_a_uuid_from_CouchDB_when_the_ReseedInterval_is_met()
        {
            // Act
            var generator = new SeededLongGenerator();
            generator.Options["ServerURL"] = _url;
            generator.Options["RestProxy"] = _mockRestProxy.Object;
            generator.Options["ReseedInterval"] = 2;
            generator.Generate(); //This call should trigger the frist uuid request
            generator.Generate();
            generator.Generate(); //This third call should trigger the next uuid request since ReseedInterval is 2

            // Assert
            _mockRestProxy.Verify(x => x.Get(_uuidURI), Times.Exactly(2));
        }

        [Test]
        public void Should_return_a_unique_non_seqential_long_integer_value_each_time_Generate_is_called()
        {
            // Act
            var generator = new SeededLongGenerator();
            generator.Options["ServerURL"] = _url;
            generator.Options["RestProxy"] = _mockRestProxy.Object;
            var firstID = generator.Generate();
            var secondID = generator.Generate();
            var thirdID = generator.Generate();

            // Assert
            Assert.AreEqual(2879578924, firstID); //these values are known because the uuid 'seed' is fixed by mocking the request
            Assert.AreEqual(1758380494, secondID);
            Assert.AreEqual(1759173619, thirdID);
        }
    }
}
