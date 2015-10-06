using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentTests.ComponentsXml.Final
{
    public class TabTag
    {
        public string Id { get; set; }
        public List<LinTag> LinTags { get; set; }

        public TabTag()
        {
            LinTags = new List<LinTag>();
        }
    }
}
