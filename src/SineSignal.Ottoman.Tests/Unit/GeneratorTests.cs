using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Net;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using SineSignal.Ottoman;
using SineSignal.Ottoman.Proxy;
using SineSignal.Ottoman.Generators;
using Moq;


namespace SineSignal.Ottoman.Tests.Unit.Generators
{
    [TestFixture]
    public class When_generating_an_ID_using_the_GUIDGenerator
    {
        [Test]
        public void Should_return_a_GUID()
        {
            var generator = new GUIDGenerator();
            var id = generator.Generate();
            Assert.IsTrue(IsGUID(id));
        }

        //GUID regex pattern taken from http://geekswithblogs.net/colinbo/archive/2006/01/18/66307.aspx
        private static bool IsGUID(string s)
        {
            string pattern = "^[A-Fa-f0-9]{32}$|" +
                             "^({|\\()?[A-Fa-f0-9]{8}-([A-Fa-f0-9]{4}-){3}[A-Fa-f0-9]{12}(}|\\))?$|" +
                             "^({)?[0xA-Fa-f0-9]{3,10}(, {0,1}[0xA-Fa-f0-9]{3,6}){2}, {0,1}({)([0xA-Fa-f0-9]{3,4}, {0,1}){7}[0xA-Fa-f0-9]{3,4}(}})$";
            if (string.IsNullOrEmpty(s) || !(new Regex(pattern)).IsMatch(s)) { return false; }
            else { return true; }   
        } 

    }

    [TestFixture]
    public class When_creating_a_HiLoGenerator
    {
        private HiLoGenerator _generator;

        [SetUp]
        public void SetUp()
        {
            _generator = new HiLoGenerator();
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
    }

    [TestFixture]
    public class When_generating_an_ID_using_the_HiLoGenerator
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
        public void Should_request_a_UUID_from_CouchDB_only_the_first_time_Generate_is_called()
        {
            // Act
            var generator = new HiLoGenerator();
            generator.Options["ServerURL"] = _url;
            generator.Options["RestProxy"] = _mockRestProxy.Object;
            generator.Generate();
            generator.Generate();
            generator.Generate();

            // Assert
            _mockRestProxy.Verify(x => x.Get(_uuidURI), Times.Once());
        }

        [Test]
        public void Should_return_the_same_uuid_with_an_incrementing_appended_value_each_time_Generate_is_called()
        {
            // Arrange
            string uuid = "0123456789abcdef0123456789abcdef";

            // Act
            var generator = new HiLoGenerator();
            generator.Options["ServerURL"] = _url;
            generator.Options["RestProxy"] = _mockRestProxy.Object;
            var firstID = generator.Generate();
            var secondID =  generator.Generate();
            var thirdID =  generator.Generate();


            Assert.AreEqual(uuid + "00000001", firstID);
            Assert.AreEqual(uuid + "00000002", secondID);
            Assert.AreEqual(uuid + "00000003", thirdID);
        }
    }
}
