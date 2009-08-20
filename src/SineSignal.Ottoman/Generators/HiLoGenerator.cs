using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SineSignal.Ottoman.Generators
{
    class HiLoGenerator : IGenerator
    {
        public Dictionary<string, string> Options { get; set; }

        public string Generate()
        {
            return Guid.NewGuid().ToString();
        }

        public HiLoGenerator()
        {
            Options = new Dictionary<string, string> { {"ServerURL", "http://127.0.0.1:5984"} };
        }
    }       
}
