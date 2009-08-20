using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using SineSignal.Ottoman.Generators;

namespace SineSignal.Ottoman.Tests.Unit
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
}
