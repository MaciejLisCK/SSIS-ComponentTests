using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentTests.ComponentsXml.Final
{
    public class LinTag
    {
        public string Num { get; set; }
        public List<FldTag> FldTags { get; set; }

        public LinTag()
        {
            FldTags = new List<FldTag>();
        }
    }
}
