using ComponentTests.ComponentsXml.X3WebService;
using Microsoft.SqlServer.Dts.Pipeline;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;

namespace ComponentTests.ComponentsXml.Final
{
    public abstract class X3WebServiceComponentBase : PipelineComponentBase
    {
        public string PublicIdentifierValue { get { return GetCustomPropertyValue("Public Identifier"); } }
        public string HostPropertyValue { get { return GetCustomPropertyValue("Host"); } }
        public int PortPropertyValue { get { return Int32.Parse(GetCustomPropertyValue("Port")); } }

        public X3WebService WebService 
        { 
            get
            {
                var webService = new X3WebService(HostPropertyValue, PortPropertyValue);

                return webService;
            }
        }

        public override void ProvideComponentProperties()
        {
            base.RemoveAllInputsOutputsAndCustomProperties();
            ComponentMetaData.RuntimeConnectionCollection.RemoveAll();

            CreateCustomProperty("Public Identifier");
            CreateCustomProperty("Host", "Host address. Could be domain or IP. Eg. google.com, 10.10.10.10");
            CreateCustomProperty("Port", "Port number");
        }

        protected X3XmlDescriptionWrapper GetWebServiceDescription()
        {
            var getDescriptionResponse = WebService.getDescription(X3WebService.Context, PublicIdentifierValue);

            var descriptionXml = new XmlDocument();
            descriptionXml.LoadXml(getDescriptionResponse.resultXml);

            var description = new X3XmlDescriptionWrapper(descriptionXml);
            return description;
        }

        protected string GenerateOutputColumnName(string groupName, string fieldName)
        {
            return groupName + "|" + fieldName;
        }

        protected void SetColumnDataType(IDTSOutputColumn100 column, string dataType, decimal length)
        {
            switch (dataType)
            {
                case "Char":
                    column.SetDataTypeProperties(DataType.DT_WSTR, (int)length, 0, 0, 0);
                    break;
                case "Integer":
                    column.SetDataTypeProperties(DataType.DT_I4, 0, 0, 0, 0);
                    break;
                case "Decimal":
                    column.SetDataTypeProperties(DataType.DT_DECIMAL, 0, 0, 0, 0);
                    break;
                case "Date":
                    column.SetDataTypeProperties(DataType.DT_DATE, 0, 0, 0, 0);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        protected void SetColumnData(PipelineBuffer buffer, int index, string value, DataType dataType)
        {
            switch (dataType)
            {
                case DataType.DT_WSTR:
                    buffer.SetString(index, value);
                    break;
                case DataType.DT_I4:
                    buffer.SetInt32(index, Int32.Parse(value));
                    break;
                case DataType.DT_DECIMAL:
                    buffer.SetDecimal(index, Decimal.Parse(value));
                    break;
                case DataType.DT_DATE:
                    buffer.SetDateTime(index, DateTime.ParseExact(value, "yyyyMMdd", CultureInfo.InvariantCulture));
                    break;
                default:
                    break;
            }
        }

        protected CAdxParamKeyValue[] GetWebServiceParameters(PipelineBuffer inputBuffer)
        {
            var parameters = new List<CAdxParamKeyValue>();
            for (int i = 0; i < InputColumns.Count; i++)
            {
                var inputColumn = InputColumns[i];
                var inputColumnIndex = GetInputCollumnIndex(inputColumn.Name);
                var inputColumnValue = inputBuffer.GetString(inputColumnIndex);

                parameters.Add(new CAdxParamKeyValue() { key = i.ToString(), value = inputColumnValue });
            }

            return parameters.ToArray();
        }
    }
}
