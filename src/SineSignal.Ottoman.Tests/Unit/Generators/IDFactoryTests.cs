#region License

// <copyright file="IDFactoryTests.cs" company="SineSignal, LLC.">
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
using Gallio.Framework;
using SineSignal.Ottoman.Generators;

namespace SineSignal.Ottoman.Tests.Unit.Generators
{
    [TestFixture]
    [Category("Unit")]
    public class When_creating_IDs_with_a_default_IDFactory : OttomanSpecBase<IDFactory>
    {
        private class TestClassWithNoIDProperty{ }

        private class TestClassWithGuidIDProperty
        {
            public Guid ID { get; set; }
        }

        private class TestClassWithIntegerIDProperty
        {
            public int ID { get; set; }
        }

        private class TestClassWithLongIntegerIDProperty
        {
            public long ID { get; set; }
        }

        private TestClassWithNoIDProperty _testObjectWithNoIDProperty;
        private TestClassWithGuidIDProperty _testObjectWithGuidIDProperty;
        private TestClassWithIntegerIDProperty _testObjectWithIntegerIDProperty; 
        private TestClassWithLongIntegerIDProperty _testObjectWithLongIntegerIDProperty;

        protected override IDFactory EstablishContext()
        {
            _testObjectWithNoIDProperty = new TestClassWithNoIDProperty();
            _testObjectWithGuidIDProperty = new TestClassWithGuidIDProperty();
            _testObjectWithIntegerIDProperty = new TestClassWithIntegerIDProperty();
            _testObjectWithLongIntegerIDProperty = new TestClassWithLongIntegerIDProperty();
            return new IDFactory();
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void Should_throw_an_exception_then_the_input_does_not_have_an_ID_property()
        {
            Sut.SetIDProperty(_testObjectWithNoIDProperty);
        }

        [Test]
        public void Should_return_a_Guid_when_ID_property_of_the_input_is_a_Guid()
        {
            Sut.SetIDProperty(_testObjectWithGuidIDProperty);
            DiagnosticLog.WriteLine("Guid ID property = {0}",_testObjectWithGuidIDProperty.ID.ToString());
        }

        [Test]
        public void Should_return_an_int_when_ID_property_of_the_input_is_an_int()
        {
            Sut.SetIDProperty(_testObjectWithIntegerIDProperty);
            DiagnosticLog.WriteLine("int ID property = {0}", _testObjectWithIntegerIDProperty.ID.ToString());
        }

        [Test]
        public void Should_return_a_long_when_ID_property_of_the_input_is_a_long()
        {
            Sut.SetIDProperty(_testObjectWithLongIntegerIDProperty);
            DiagnosticLog.WriteLine("long ID property = {0}", _testObjectWithLongIntegerIDProperty.ID.ToString());
        }
    }
}
