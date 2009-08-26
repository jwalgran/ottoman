﻿#region License

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
using System.Security.Cryptography;

namespace SineSignal.Ottoman.Generators
{
	/// <summary>
	/// Used for generating non-sequential identifiers of type Int64.
	/// </summary>
    public class RandomLongIntegerGenerator : IGenerator<long>
    {
        private static RandomNumberGenerator _randomNumberGenerator = RandomNumberGenerator.Create();

		/// <summary>
		/// Gets or sets a value indicating whether to allow negative identifiers.
		/// </summary>
		/// <value><c>true</c> if [allow negative]; otherwise, <c>false</c>.</value>
        public bool AllowNegative { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="RandomLongIntegerGenerator"/> class.
		/// </summary>
        public RandomLongIntegerGenerator() : this(false) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="RandomLongIntegerGenerator"/> class.
		/// </summary>
		/// <param name="allowNegative">if set to <c>true</c> [allow negative].</param>
        public RandomLongIntegerGenerator(bool allowNegative)
        {
            AllowNegative = allowNegative;
        }

		/// <summary>
		/// Generates a unique document identifier.
		/// </summary>
		/// <returns>A unique integer each time the function is called.</returns>
        public long Generate()
        {
            byte[] randomBytes = new byte[8];
            _randomNumberGenerator.GetBytes(randomBytes);
            
            if (AllowNegative)
            {
				return BitConverter.ToInt64(randomBytes, 0);
            }
            
			return BitConverter.ToUInt32(randomBytes,0);
        }
    }
}