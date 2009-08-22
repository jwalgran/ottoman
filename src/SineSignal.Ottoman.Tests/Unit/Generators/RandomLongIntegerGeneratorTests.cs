#region License

// <copyright file="RandomLongIntegerGeneratorTests.cs" company="SineSignal, LLC.">
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
using SineSignal.Ottoman.Generators;
using System.Collections.Generic;

namespace SineSignal.Ottoman.Tests.Unit.Generators
{
    [TestFixture]
    public class When_generating_an_identifier_using_the_RandomLongIntegerGenerator
    {
        private RandomLongIntegerGenerator _generator;

        [SetUp]
        public void SetUp() 
        {
            _generator = new RandomLongIntegerGenerator();
        }

        [Test]
        public void Should_return_a_long_integer()
        {
            var id = _generator.Generate();
            Assert.IsInstanceOfType<long>(id);
        }

        [Test]
        void Should_return_a_unique_long_integer_each_time_Generate_is_called()
        {
            var idHash = new Dictionary<long, long> { };
            for (int i = 0; i < 10000; i++)
            {
                var id = _generator.Generate();
                if (idHash.ContainsKey(id))
                    Assert.Fail("An identifier was repeated");
                else
                    idHash.Add(id,id);
            }
        }

        [Test]
        void Should_not_return_negative_numbers_unless_AllowNegative_option_is_true()
        {
            _generator.Options["AllowNegative"] = false;
            for (int i = 0; i < 10000; i++)
            {
                Assert.LessThanOrEqualTo<long>(0, _generator.Generate());
            }
        }
    }
}
