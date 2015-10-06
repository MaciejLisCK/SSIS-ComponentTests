using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ComponentTests.ComponentsXml.X3WebService
{
    public abstract class XmlWrapperObject
    {
        protected XmlDocument XmlDocument { get; private set; }

        public XmlWrapperObject(XmlDocument xmlDocument)
        {
            XmlDocument = xmlDocument;
        }

        protected XmlNodeList GetNodes(string nodePath)
        {
            var nodes = GetNodes(nodePath, XmlDocument.DocumentElement);

            return nodes;
        }

        protected XmlNodeList GetNodes(string nodePath, XmlNode parentNode)
        {
            var nodes = parentNode.SelectNodes(nodePath);

            return nodes;
        }

        protected XmlNode GetNode(string nodePath)
        {
            var node = GetNode(nodePath, XmlDocument.DocumentElement);

            return node;
        }

        protected XmlNode GetNode(string nodePath, XmlNode parentNode)
        {
            var node = parentNode.SelectSingleNode(nodePath);

            return node;
        }

        protected string GetInnerText(string nodePath)
        {
            var nodeInnerText = GetInnerText(nodePath, XmlDocument.DocumentElement);

            return nodeInnerText;
        }

        protected string GetInnerText(string nodePath, XmlNode parentNode)
        {
            var node = GetNode(nodePath, parentNode);
            var nodeInnerText = node.InnerText;

            return nodeInnerText;
        }

        protected T GetValue<T>(string nodePath)
        {
            var resultValue = GetValue<T>(nodePath, XmlDocument.DocumentElement);

            return resultValue;
        }

        protected T GetValue<T>(string nodePath, XmlNode parentNode)
        {
            var resultType = typeof(T);
            var resultTypeCode = Type.GetTypeCode(resultType);

            var nodeInnerText = GetInnerText(nodePath, parentNode);

            switch (resultTypeCode)
            {
                case TypeCode.String:
                    return (T)(nodeInnerText as object);

                case TypeCode.Boolean:
                    var booleanValue = Boolean.Parse(nodeInnerText);
                    return (T)(booleanValue as object);

                case TypeCode.Int32:
                    var int32Value = Int32.Parse(nodeInnerText);
                    return (T)(int32Value as object);

                case TypeCode.DateTime:
                    var dateTimeValue = DateTime.ParseExact(nodeInnerText, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    return (T)(dateTimeValue as object);

                case TypeCode.Decimal:
                    var decimalValue = Decimal.Parse(nodeInnerText);
                    return (T)(decimalValue as object);

                default:
                    throw new NotImplementedException("Convertion to type: " + resultType.FullName + " is not implemented.");
            }
        }
    }
}
