﻿using System;
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