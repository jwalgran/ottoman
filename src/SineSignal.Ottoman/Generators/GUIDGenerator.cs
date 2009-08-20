﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SineSignal.Ottoman.Generators
{
    class GUIDGenerator : IGenerator
    {
        public Dictionary<string, string> Options { get; set; }
        
        public string Generate()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
