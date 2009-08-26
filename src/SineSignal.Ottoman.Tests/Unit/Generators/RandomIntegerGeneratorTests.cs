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

using System.Collections.Generic;
using System.Linq;

using MbUnit.Framework;

using SineSignal.Ottoman.Generators;

namespace SineSignal.Ottoman.Tests.Unit.Generators
{
    [TestFixture]
	[Category("Unit")]
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
        public void Should_return_a_unique_integer_each_time_Generate_is_called()
        {
			HashSet<int> idHash = new HashSet<int>();
			for (int i = 0; i < 10; i++)
			{
				var id = Sut.Generate();
				bool wasAdded = idHash.Add(id);

				if (!wasAdded)
				{
					Assert.Fail("An identifier was repeated");
				}
			}
        }

        [Test]
        public void Should_not_return_negative_numbers_when_AllowNegative_is_false()
        {
            for (int i = 0; i < 10000; i++)
            {
                Assert.LessThanOrEqualTo<int>(0, Sut.Generate());
            }
        }
    }
    
    [TestFixture]
	[Category("Unit")]
    public class When_generating_an_identifier_using_the_RandomIntegerGenerator_when__allow_negative_is_true : OttomanSpecBase<RandomIntegerGenerator>
    {
		protected override RandomIntegerGenerator EstablishContext()
		{
			return new RandomIntegerGenerator(true);
		}

		[Test]
		public void Should_return_an_integer()
		{
			Assert.IsInstanceOfType<int>(Sut.Generate());
		}

		[Test]
		public void Should_return_a_unique_integer_each_time_Generate_is_called()
		{
			HashSet<int> idHash = new HashSet<int>();
			for (int i = 0; i < 10; i++)
			{
				var id = Sut.Generate();
				bool wasAdded = idHash.Add(id);
				
				if (!wasAdded)
				{
					Assert.Fail("An identifier was repeated");
				}
			}
		}

		[Test]
		public void Should_return_negative_numbers_when_AllowNegative_is_true()
		{
			HashSet<int> idHash = new HashSet<int>();
			for (int i = 0; i < 10000; i++)
			{
				int id = Sut.Generate();
				idHash.Add(id);
			}

			IList<int> negatives = idHash.Where(x => x < 0).ToList();

			if (negatives.Count < 0)
			{
				Assert.Fail("Never returned a single negative value.");
			}
		}
    }
}
