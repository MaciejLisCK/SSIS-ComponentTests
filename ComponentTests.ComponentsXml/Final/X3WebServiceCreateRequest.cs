using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentTests.ComponentsXml.Final
{
    public class X3WebServiceCreateRequest
    {
        public List<GroupTag> GrpTags { get; set; }
        public List<TabTag> TabTags { get; set; }

        public X3WebServiceCreateRequest()
        {
            GrpTags = new List<GroupTag>();
            TabTags = new List<TabTag>();
        }
    }
}
