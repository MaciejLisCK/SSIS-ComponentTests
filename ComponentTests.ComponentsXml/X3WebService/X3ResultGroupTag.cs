using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentTests.ComponentsXml.X3WebService
{
    public class X3ResultGroupTag
    {
        public string Id { get; set; }
        public IEnumerable<X3ResultFieldTag> Fields { get; set; }
    }
}
