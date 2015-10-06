using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ComponentTests.ComponentsXml.X3WebService
{
    public class X3ListResponseWrapper : XmlWrapperObject
    {
        public List<X3ListLine> Lines
        {
            get
            {
                var listOfResponses = GetNodes("/RESULT/LIN").Cast<XmlNode>();

                return Mapper.Map<List<X3ListLine>>(listOfResponses);
            }
        }

        public X3ListResponseWrapper(XmlDocument xmlDocument)
            : base(xmlDocument)
        { }
    }
}
