using AutoMapper;
using ComponentTests.ComponentsXml.X3WebService;
using Microsoft.SqlServer.Dts.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ComponentTests.ComponentsXml.Final
{
    [DtsPipelineComponent(DisplayName = "X3WebServiceReadDev", ComponentType = ComponentType.SourceAdapter,
        IconResource = "ComponentTests.ComponentsXml.Resources.Icon1.ico")]
    public class X3WebServiceReadComponent : X3WebServiceComponentBase
    {
        private PipelineBuffer outputBuffer;

        public X3WebServiceReadComponent()
        {
            Mapper.CreateMap<XmlNode, X3DescriptionFieldTag>()
                .ForMember(dm => dm.Name, mo => mo.MapFrom(sm => sm.Attributes["NAM"].Value))
                .ForMember(dm => dm.DataType, mo => mo.MapFrom(sm => sm.Attributes["TYP"].Value))
                .ForMember(dm => dm.Length, mo => mo.MapFrom(sm => Decimal.Parse(sm.Attributes["LEN"].Value)));

            Mapper.CreateMap<XmlNode, X3DescriptionGroupTag>()
                .ForMember(dm => dm.Name, mo => mo.MapFrom(sm => sm.Attributes["NAM"].Value))
                .ForMember(dm => dm.Fields, mo => mo.MapFrom(sm => sm.SelectNodes("FLD").Cast<XmlNode>()));

            Mapper.CreateMap<XmlNode, X3ResultGroupTag>()
                .ForMember(dm => dm.Id, mo => mo.MapFrom(sm => sm.Attributes["ID"].Value))
                .ForMember(dm => dm.Fields, mo => mo.MapFrom(sm => sm.SelectNodes("FLD").Cast<XmlNode>()));

            Mapper.CreateMap<XmlNode, X3ResultFieldTag>()
                .ForMember(dm => dm.Name, mo => mo.MapFrom(sm => sm.Attributes["NAME"].Value))
                .ForMember(dm => dm.Value, mo => mo.MapFrom(sm => sm.InnerText));
        }

        public override void ReinitializeMetaData()
        {
            var description = GetWebServiceDescription();

            bool hasInputCollection = ComponentMetaData.InputCollection.Count > 0;
            if(!hasInputCollection)
            {
                var inputCollection = ComponentMetaData.InputCollection.New();
                inputCollection.Name = "Input";
            }

            bool hasOutputCollection = ComponentMetaData.OutputCollection.Count > 0;
            if (!hasOutputCollection)
            {
                var outputCollection = ComponentMetaData.OutputCollection.New();
                outputCollection.Name = "Output";

                foreach (var groupTag in description.ObjectDescription)
                {
                    foreach (var fieldTag in groupTag.Fields)
                    {
                        var columnName = GenerateOutputColumnName(groupTag.Name, fieldTag.Name);
                        var newColumn = outputCollection.OutputColumnCollection.New();

                        newColumn.Name = columnName;

                        SetColumnDataType(newColumn, fieldTag.DataType, fieldTag.Length);
                    }
                }
            }
        }

        public override void PrimeOutput(int outputs, int[] outputIDs, PipelineBuffer[] buffers)
        {
            outputBuffer = buffers.First();
        }

        public override void ProcessInput(int inputID, PipelineBuffer inputBuffer)
        {
            while (inputBuffer.NextRow())
            {
                var parameters = GetWebServiceParameters(inputBuffer);

                var response = WebService.read(X3WebService.Context, PublicIdentifierValue, parameters);

                var xml = new XmlDocument();
                xml.LoadXml(response.resultXml);

                var responseXmlWrapper = new X3ObjectResponseWrapper(xml);

                outputBuffer.AddRow();
                foreach (var groupTag in responseXmlWrapper.ResultGroups)
                {
                    foreach (var fieldTag in groupTag.Fields)
                    {
                        var columnName = GenerateOutputColumnName(groupTag.Id, fieldTag.Name);
                        var outputCollumnIndex = GetOutputColumnIndex(columnName);
                        var outputColumn = GetOutputColumn(columnName);

                        SetColumnData(outputBuffer, outputCollumnIndex, fieldTag.Value, outputColumn.DataType);
                    }
                }
            }

            if (inputBuffer.EndOfRowset)
                outputBuffer.SetEndOfRowset();
        }
    }
}
