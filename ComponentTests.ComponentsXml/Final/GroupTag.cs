using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentTests.ComponentsXml.Final
{
    public class GroupTag
    {
        public string Id { get; set; }
        public List<FldTag> FldTags { get; set; }

        public GroupTag()
        {
            FldTags = new List<FldTag>();
        }
    }
}
