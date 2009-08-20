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
