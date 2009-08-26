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

using System.Collections.Generic;
using System.Linq;

using MbUnit.Framework;

using SineSignal.Ottoman.Generators;

namespace SineSignal.Ottoman.Tests.Unit.Generators
{
	[TestFixture]
	[Category("Unit")]
	public class When_generating_an_identifier_using_the_RandomLongIntegerGenerator : OttomanSpecBase<RandomLongIntegerGenerator>
	{
		protected override RandomLongIntegerGenerator EstablishContext()
		{
			return new RandomLongIntegerGenerator();
		}

		[Test]
		public void Should_return_a_long_integer()
		{
			Assert.IsInstanceOfType<long>(Sut.Generate());
		}

		[Test]
		public void Should_return_a_unique_long_integer_each_time_Generate_is_called()
		{
			HashSet<long> idHash = new HashSet<long>();
			for (int i = 0; i < 10000; i++)
			{
				long id = Sut.Generate();
				bool added = idHash.Add(id);

				if (!added)
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
				Assert.LessThanOrEqualTo(0, Sut.Generate());
			}
		}
	}

	[TestFixture]
	[Category("Unit")]
	public class When_generating_an_identifier_using_the_RandomLongIntegerGenerator_when__allow_negative_is_true : OttomanSpecBase<RandomLongIntegerGenerator>
	{
		protected override RandomLongIntegerGenerator EstablishContext()
		{
			return new RandomLongIntegerGenerator(true);
		}

		[Test]
		public void Should_return_a_long_integer()
		{
			Assert.IsInstanceOfType<long>(Sut.Generate());
		}

		[Test]
		public void Should_return_a_unique_long_integer_each_time_Generate_is_called()
		{
			HashSet<long> idHash = new HashSet<long>();
			for (int i = 0; i < 10000; i++)
			{
				long id = Sut.Generate();
				bool added = idHash.Add(id);

				if (!added)
				{
					Assert.Fail("An identifier was repeated");
				}
			}
		}

		[Test]
		public void Should_return_negative_numbers_when_AllowNegative_is_true()
		{
			HashSet<long> idHash = new HashSet<long>();
			for (int i = 0; i < 10000; i++)
			{
				long id = Sut.Generate();
				idHash.Add(id);
			}

			IList<long> negatives = idHash.Where(x => x < 0).ToList();
			
			if (negatives.Count < 0)
			{
				Assert.Fail("Never returned a single negative value.");
			}
		}
	}
}