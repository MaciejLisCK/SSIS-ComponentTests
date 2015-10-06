using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentTests.ComponentsXml.X3WebService
{
    [DebuggerDisplay("{Name} [{DataType}]")]
    public class X3DescriptionFieldTag
    {
        public string Name { get; set; }
        public string DataType { get; set; }
        public decimal Length { get; set; }
    }
}
