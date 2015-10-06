using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ComponentTests.ComponentsXml.Final
{
    public class X3WebServiceCreateRequestParser
    {
        public string ParseToXml(X3WebServiceCreateRequest request)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            var result = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(result, settings);

            writer.WriteStartElement("PARAM");
            foreach (var grpTag in request.GrpTags)
            {
                writer.WriteStartElement("GRP");
                writer.WriteAttributeString("ID", grpTag.Id);

                foreach (var fldTag in grpTag.FldTags)
                {
                    writer.WriteStartElement("FLD");
                    writer.WriteAttributeString("NAME", fldTag.Name);
                    writer.WriteString(fldTag.Value);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
            foreach (var tabTag in request.TabTags)
            {
                writer.WriteStartElement("TAB");
                writer.WriteAttributeString("ID", tabTag.Id);
                writer.WriteAttributeString("SIZE", tabTag.LinTags.Count.ToString());


                foreach (var linTag in tabTag.LinTags)
                {
                    writer.WriteStartElement("LIN");
                    writer.WriteAttributeString("NUM", linTag.Num.ToString());

                    foreach (var fldTag in linTag.FldTags)
                    {
                        writer.WriteStartElement("FLD");
                        writer.WriteAttributeString("NAME", fldTag.Name);
                        writer.WriteString(fldTag.Value);
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            writer.Close();

            return result.ToString();
        }

    }
}
