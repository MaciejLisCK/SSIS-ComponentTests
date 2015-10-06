using Microsoft.SqlServer.Dts.Pipeline;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComponentTests.ComponentsXml.X3WebService;
using System.Xml;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;
using System.Globalization;
using AutoMapper;

namespace ComponentTests.ComponentsXml.Final
{
    [DtsPipelineComponent(DisplayName = "X3WebServiceListDev", ComponentType = ComponentType.SourceAdapter,
        IconResource = "ComponentTests.ComponentsXml.Resources.Icon1.ico")]
    public class X3WebServiceListComponent : X3WebServiceComponentBase
    {
        public int ListSizePropertyValue { get { return Int32.Parse(GetCustomPropertyValue("List Size")); } }

        public X3WebServiceListComponent()
        {
            Mapper.CreateMap<XmlNode, X3DescriptionFieldTag>()
                .ForMember(dm => dm.Name, mo => mo.MapFrom(sm => sm.Attributes["NAM"].Value))
                .ForMember(dm => dm.DataType, mo => mo.MapFrom(sm => sm.Attributes["TYP"].Value))
                .ForMember(dm => dm.Length, mo => mo.MapFrom(sm => Decimal.Parse(sm.Attributes["LEN"].Value)))
                ;

            Mapper.CreateMap<XmlNode, X3DescriptionGroupTag>()
                .ForMember(dm => dm.Fields, mo => mo.MapFrom(sm => sm.SelectNodes("FLD").Cast<XmlNode>()));


            Mapper.CreateMap<XmlNode, X3ListField>()
                .ForMember(dm => dm.Name, mo => mo.MapFrom(sm => sm.Attributes["NAME"].Value))
                .ForMember(dm => dm.DataType, mo => mo.MapFrom(sm => sm.Attributes["TYPE"].Value))
                .ForMember(dm => dm.Value, mo => mo.MapFrom(sm => sm.InnerText))
                ;

            Mapper.CreateMap<XmlNode, X3ListLine>()
                .ForMember(dm => dm.Fields, mo => mo.MapFrom(sm => sm.SelectNodes("FLD").Cast<XmlNode>()));
        }

        public override void ProvideComponentProperties()
        {
            base.ProvideComponentProperties();

            CreateCustomProperty("List Size", "Maximum numbers of rows to receive.");
        }

        public override void ReinitializeMetaData()
        {
            var description = GetWebServiceDescription();

            bool hasOutputCollection = ComponentMetaData.OutputCollection.Count > 0;
            if (!hasOutputCollection)
            {
                var outputCollection = ComponentMetaData.OutputCollection.New();
                outputCollection.Name = "Output";

                foreach (var groupTag in description.ListDescription)
                {
                    foreach (var fieldTag in groupTag.Fields)
                    {
                        var columnName = fieldTag.Name;
                        var newColumn = outputCollection.OutputColumnCollection.New();

                        newColumn.Name = columnName;

                        SetColumnDataType(newColumn, fieldTag.DataType, fieldTag.Length);
                    }
                }
            }
        }

        public override void PrimeOutput(int outputs, int[] outputIDs, PipelineBuffer[] buffers)
        {
            var outputBuffer = buffers[0];

            var parameters = new List<CAdxParamKeyValue>();
            var response = WebService.query(X3WebService.Context, PublicIdentifierValue, parameters.ToArray(), 10);

            var xml = new XmlDocument();
            xml.LoadXml(response.resultXml);

            var responseXmlWrapper = new X3ListResponseWrapper(xml);

            foreach (var lineTag in responseXmlWrapper.Lines)
            {
                outputBuffer.AddRow();

                foreach (var fieldTag in lineTag.Fields)
                {
                    var outputCollumnIndex = GetOutputColumnIndex(fieldTag.Name);
                    var outputColumn = GetOutputColumn(fieldTag.Name);

                    SetColumnData(outputBuffer, outputCollumnIndex, fieldTag.Value, outputColumn.DataType);
                }
            }

            outputBuffer.SetEndOfRowset();
        }
    }
}