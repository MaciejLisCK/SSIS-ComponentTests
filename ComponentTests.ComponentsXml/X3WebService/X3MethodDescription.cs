using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ComponentTests.ComponentsXml.X3WebService
{
    public class X3XmlDescriptionWrapper : XmlWrapperObject
    {
        public X3XmlDescriptionWrapper(XmlDocument xmlDocument)
            : base(xmlDocument)
        { }

        public IList<X3DescriptionGroupTag> ListDescription
        {
            get
            {
                var nodesList = this.GetNodes("/ADXDOC/ADXKEY/GRP");
                var nodes = nodesList.Cast<XmlNode>();
                var groups = nodes
                    .Select(n => Mapper.Map<X3DescriptionGroupTag>(n));

                return groups.ToList();
            }
        }
        public IList<X3DescriptionGroupTag> ObjectDescription
        {
            get
            {
                var nodesList = this.GetNodes("/ADXDOC/ADXDATA/GRP");
                var nodes = nodesList.Cast<XmlNode>();
                var groups = nodes
                    .Select(n => Mapper.Map<X3DescriptionGroupTag>(n));

                return groups.ToList();
            }
        }

        public IList<X3DescriptionGroupTag> ReadDescription
        {
            get
            {
                var nodesList = this.GetNodes("/ADXDOC/ADXREAD/GRP");
                var nodes = nodesList.Cast<XmlNode>();
                var groups = nodes
                    .Select(n => Mapper.Map<X3DescriptionGroupTag>(n));

                return groups.ToList();
            }
        }
    }
}
