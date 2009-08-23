#region License
// <copyright file="RandomLongIntegerGenerator.cs" company="SineSignal, LLC.">
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
using System.Security.Cryptography;

namespace SineSignal.Ottoman.Generators
{
    public class RandomLongIntegerGenerator : IGenerator<long>
    {
        private static RandomNumberGenerator _randomNumberGenerator = RandomNumberGenerator.Create();

        public bool AllowNegative { get; set; }

        public RandomLongIntegerGenerator() : this(false) { }

        public RandomLongIntegerGenerator(bool allowNegative)
        {
            AllowNegative = allowNegative;
        }

        public long Generate()
        {
            byte[] randomBytes = new byte[8];
            _randomNumberGenerator.GetBytes(randomBytes);
            if (AllowNegative)
                return BitConverter.ToInt64(randomBytes,0);
            else
                return (long)BitConverter.ToUInt32(randomBytes,0);

        }
    }
}