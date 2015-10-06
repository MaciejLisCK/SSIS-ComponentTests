using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentTests.ComponentsXml.X3WebService
{
    [DebuggerDisplay("{Name} = {Value}")]
    public class X3ListField
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string DataType { get; set; }
    }
}
