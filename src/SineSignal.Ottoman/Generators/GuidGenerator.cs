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

namespace SineSignal.Ottoman.Generators
{
    /// <summary>
    /// Used for generating random document IDs using the .NET Framework Guid class.
    /// </summary>
    class GuidGenerator : IGenerator<Guid>
    {
        public Dictionary<string, object> Options { get; set; }

        /// <summary>
        /// Creates a unique, random document ID.
        /// </summary>
        /// <returns>The default string representation of a new Guid.</returns>
        public Guid Generate()
        {
            return Guid.NewGuid();
        }
    }
}
