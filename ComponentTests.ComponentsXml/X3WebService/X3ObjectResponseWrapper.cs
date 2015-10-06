using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ComponentTests.ComponentsXml.X3WebService
{
    public class X3ObjectResponseWrapper : XmlWrapperObject
    {
        public IEnumerable<X3ResultGroupTag> ResultGroups
        { 
            get 
            {
                var groupNodes = GetNodes("/RESULT/GRP");

                var result = Mapper.Map<IEnumerable<X3ResultGroupTag>>(groupNodes);

                return result;
            } 
        }

        public X3ObjectResponseWrapper(XmlDocument xmlDocument)
            : base(xmlDocument)
        { }
    }
}
