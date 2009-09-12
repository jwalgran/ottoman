#region License

// <copyright file="IDFactory.cs" company="SineSignal, LLC.">
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
using System.Reflection;

namespace SineSignal.Ottoman.Generators
{
    public class IDFactory : IIDFactory
    {
        private Dictionary<Type, object> _generatorDictionary = new Dictionary<Type,object>();

        public IDFactory()
        {
            _generatorDictionary.Add(typeof(Guid), new GuidGenerator());
            _generatorDictionary.Add(typeof(long), new RandomLongIntegerGenerator());
            _generatorDictionary.Add(typeof(int), new RandomIntegerGenerator());
        }

        private object CreateID(Type idType)
        {
            if (!_generatorDictionary.ContainsKey(idType))
            {
                throw new Exception("No generator registered for type " + idType.Name);
            }

            var generator = _generatorDictionary[idType];
            var generateMethod = generator.GetType().GetMethod("Generate");
            return generateMethod.Invoke(generator, null);
        }

        public void SetIDProperty(object item)
        {
            var propertyInfos = item.GetType().GetProperties();
            var idProperty = propertyInfos.Single(x => x.Name == "ID");
            idProperty.SetValue(item, CreateID(idProperty.PropertyType),null);
        }
    }
}
