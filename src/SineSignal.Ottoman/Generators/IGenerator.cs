using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SineSignal.Ottoman.Generators
{
    public interface IGenerator
    {
        Dictionary<string, string> Options { get; set; }
        string Generate();
    }
}
