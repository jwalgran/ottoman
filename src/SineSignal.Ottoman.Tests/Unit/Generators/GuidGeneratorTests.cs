using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SineSignal.Ottoman.Generators;
using MbUnit.Framework;
using Moq;

namespace SineSignal.Ottoman.Tests.Unit.Generators.GuidGeneratorTests
{
    [TestFixture]
    public class When_generating_an_ID_using_the_GuidGenerator
    {
        [Test]
        public void Should_return_a_Guid()
        {
            var generator = new GuidGenerator();
            var id = generator.Generate();
            Assert.IsInstanceOfType<Guid>(id);
        }
    }
}
