#region License
// <copyright file="RandomIntegerGenerator.cs" company="SineSignal, LLC.">
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
    class RandomIntegerGenerator : IGenerator<int>
    {
        private RandomNumberGenerator _randomNumberGenerator = RandomNumberGenerator.Create();
        public bool AllowNegative { get; set; }

        public Dictionary<string, object> Options { get; set; }
        public int Generate()
        {
            byte[] randomBytes = new byte[8];
            _randomNumberGenerator.GetBytes(randomBytes);
            if (AllowNegative)
                return BitConverter.ToInt32(randomBytes, 0);
            else
                return (int)BitConverter.ToUInt16(randomBytes, 0);

        }

        public RandomIntegerGenerator(): this(false){}

        public RandomIntegerGenerator(bool allowNegative)
        {
           AllowNegative = allowNegative;
        }
    }
}