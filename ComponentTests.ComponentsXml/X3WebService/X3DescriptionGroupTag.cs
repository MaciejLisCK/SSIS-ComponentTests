using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentTests.ComponentsXml.X3WebService
{
    public class X3DescriptionGroupTag
    {
        public string Name { get; set; }
        public string Tyb { get; set; }
        public IList<X3DescriptionFieldTag> Fields { get; set; }
    }
}
