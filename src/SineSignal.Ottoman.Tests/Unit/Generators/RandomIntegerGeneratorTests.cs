#region License

// <copyright file="RandomIntegerGeneratorTests.cs" company="SineSignal, LLC.">
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
    public class When_generating_an_identifier_using_the_RandomIntegerGenerator : OttomanSpecBase<RandomIntegerGenerator>
    {
        protected override RandomIntegerGenerator EstablishContext()
        {
            return new RandomIntegerGenerator();
        }

        [Test]
        public void Should_return_an_integer()
        {
            Assert.IsInstanceOfType<int>(Sut.Generate());
        }

        [Test]
        void Should_return_a_unique_integer_each_time_Generate_is_called()
        {
            var idHash = new Dictionary<int, int> { };
            for (int i = 0; i < 10; i++)
            {
                var id = Sut.Generate();
                if (idHash.ContainsKey(id))
                    Assert.Fail("An identifier was repeated");
                else
                    idHash.Add(id, id);
            }
        }

        [Test]
        void Should_not_return_negative_numbers_when_AllowNegative_is_false()
        {
            Sut.AllowNegative = false;
            for (int i = 0; i < 10000; i++)
            {
                Assert.LessThanOrEqualTo<int>(0, Sut.Generate());
            }
        }
    }
}
