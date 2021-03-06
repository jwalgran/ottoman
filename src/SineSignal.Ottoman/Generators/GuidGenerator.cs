﻿#region License

// <copyright file="GuidGenerator.cs" company="SineSignal, LLC.">
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

namespace SineSignal.Ottoman.Generators
{
    /// <summary>
    /// Used for generating random document identifiers using the .NET Framework Guid class.
    /// </summary>
    public class GuidGenerator : IGenerator<Guid>
    {
        /// <summary>
        /// Creates a unique, random document identifier.
        /// </summary>
        /// <returns>The new Guid.</returns>
        public Guid Generate()
        {
            return Guid.NewGuid();
        }
    }
}
