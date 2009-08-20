using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SineSignal.Ottoman.Generators
{
    public interface IGenerator<T>
    {
        Dictionary<string, object> Options { get; set; }
        T Generate();
    }
}