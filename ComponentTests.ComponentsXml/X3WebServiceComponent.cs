using AutoMapper;
using ComponentTests.ComponentsXml.X3WebService;
using Microsoft.SqlServer.Dts.Pipeline;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ComponentTests.ComponentsXml
{
    [DtsPipelineComponent(DisplayName = "X3WebService", ComponentType = ComponentType.SourceAdapter,
        IconResource = "ComponentTests.ComponentsXml.Resources.Icon1.ico")]
    public class X3WebServiceComponent : PipelineComponent
    {
        private const string publicIndentifierPropertyName = "Public Identifier";
        private int xmlOutputCollumnIndex;
        private string lastPublicIdentifierValue = String.Empty; 
        public string CodeLang { get; set; }
        public string CodeUser { get; set; }
        public string Password { get; set; }
        public string PoolAlias { get; set; }
        public string RequestConfig { get; set; }

        private string PublicIdentifier
        {
            get
            {
                var customProperties = ComponentMetaData.CustomPropertyCollection.Cast<IDTSCustomProperty100>();
                var publicIdentifierProperty = customProperties.Single(cp => cp.Name == publicIndentifierPropertyName);

                return publicIdentifierProperty.Value;
            }
        }

        private string Ip
        {
            get
            {
                var customProperties = ComponentMetaData.CustomPropertyCollection.Cast<IDTSCustomProperty100>();
                var publicIdentifierProperty = customProperties.Single(cp => cp.Name == "IP");

                return publicIdentifierProperty.Value;
            }
        }

        private CAdxCallContext CallContext
        {
            get
            {
                var context = new CAdxCallContext();
                context.codeLang = CodeLang;
                context.codeUser = CodeUser;
                context.password = Password;
                context.poolAlias = PoolAlias;
                context.requestConfig = RequestConfig;

                return context;
            }
        }

        public X3WebServiceComponent()
        {
            CodeLang = "ENG";
            CodeUser = "ADMWS";
            Password = "";
            PoolAlias = "PROTTSG";
            RequestConfig = "adxwss.trace.on=off&adxwss.trace.size=0&adonix.trace.on=off&adonix.trace.level=3&adonix.trace.size=0";

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

            var publicIdentifierProperty = ComponentMetaData.CustomPropertyCollection.New();
            publicIdentifierProperty.Description = "Web Service method";
            publicIdentifierProperty.Name = publicIndentifierPropertyName;
            publicIdentifierProperty.Value = lastPublicIdentifierValue;

            var ipProperty = ComponentMetaData.CustomPropertyCollection.New();
            ipProperty.Description = "IP";
            ipProperty.Name = "IP";
            ipProperty.Value = String.Empty;
            ipProperty.ExpressionType = DTSCustomPropertyExpressionType.CPET_NOTIFY;

            /*
            ComponentMetaData.OutputCollection.New();
            IDTSOutput100 output = ComponentMetaData.OutputCollection.New();
            output.Name = "Output";
            var outputColumns = output.OutputColumnCollection;
            var newColumn = outputColumns.New();
            newColumn.Name = "XML";
            newColumn.SetDataTypeProperties(DataType.DT_WSTR, 4000, 0, 0, 0);
            */
            
            /*
            IDTSCustomProperty100 webServiceMethodProperty = ComponentMetaData.CustomPropertyCollection.New();
            webServiceMethodProperty.Description = "Method of WebService, could be query, read, etc.";
            webServiceMethodProperty.Name = "Method";
            webServiceMethodProperty.Value = String.Empty;

            IDTSCustomProperty100 webServiceParameter0Property = ComponentMetaData.CustomPropertyCollection.New();
            webServiceParameter0Property.Description = "First Parameter for call.";
            webServiceParameter0Property.Name = "Parameter0";
            webServiceParameter0Property.Value = String.Empty;
             */ 

            //IDTSRuntimeConnection100 connection = ComponentMetaData.RuntimeConnectionCollection.New();
            //connection.Name = "ADO.NET";
        }

        public override DTSValidationStatus Validate()
        {
            var customProperties = ComponentMetaData.CustomPropertyCollection.Cast<IDTSCustomProperty100>();
            var publicIdentifierProperty = customProperties.Single(cp => cp.Name == publicIndentifierPropertyName);

            if (String.IsNullOrWhiteSpace(publicIdentifierProperty.Value))
                return DTSValidationStatus.VS_ISBROKEN;

            bool hasOutput = ComponentMetaData.OutputCollection.Count > 0;
            bool isLastPublicIndentifierEmpty = String.IsNullOrWhiteSpace(lastPublicIdentifierValue);
            bool hasPublicIdentifierValueChanged = publicIdentifierProperty.Value != lastPublicIdentifierValue;
            if ((!hasOutput && hasPublicIdentifierValueChanged) || (hasOutput && hasPublicIdentifierValueChanged && !isLastPublicIndentifierEmpty) )
            {
                lastPublicIdentifierValue = publicIdentifierProperty.Value;
                return DTSValidationStatus.VS_NEEDSNEWMETADATA;
            }

            return base.Validate();
        }

        private void GenerateOutputs()
        {
            var binding = new BasicHttpBinding();
            binding.MaxReceivedMessageSize = 999999999;

            var proxy = new CAdxWebServiceXmlCCClient(binding, new EndpointAddress("http://"+Ip+":28880/adxwsvc/services/CAdxWebServiceXmlCC"));
            var parameters = new List<CAdxParamKeyValue>();
            var response = proxy.getDescription(CallContext, PublicIdentifier);

            var descriptionRawXml = response.resultXml;
            var descriptionXml = new XmlDocument();
            descriptionXml.LoadXml(descriptionRawXml);

            var description = new X3XmlDescriptionWrapper(descriptionXml);

            IDTSOutput100 output;
            bool hasOutputCollection = ComponentMetaData.OutputCollection.Count > 0;
            if (!hasOutputCollection)
            {
                output = ComponentMetaData.OutputCollection.New();
                output.Name = "Output";
            }
            else
                output = ComponentMetaData.OutputCollection[0];

            var outputColumns = output.OutputColumnCollection;
            var outputColumnsCasted = outputColumns.Cast<IDTSOutputColumn100>();
            /*
            foreach (var listField in description.ListDescription)
            {
                IDTSOutputColumn100 newColumn;
                if (outputColumnsCasted.Any(c => c.Name == listField.Name))
                    newColumn = outputColumnsCasted.Single(c => c.Name == listField.Name);
                else
                    newColumn = outputColumns.New();

                newColumn.Name = listField.Name;
                switch (listField.DataType)
                {
                    case "Char":
                        newColumn.SetDataTypeProperties(DataType.DT_WSTR, (int)listField.Length, 0, 0, 0);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }*/
        }

        public override void ReinitializeMetaData()
        {
            base.ReinitializeMetaData();

            GenerateOutputs();
        }

        public override void PreExecute()
        {
            IDTSOutput100 output = ComponentMetaData.OutputCollection[0];

            xmlOutputCollumnIndex = (int)BufferManager.FindColumnByLineageID(output.Buffer, output.OutputColumnCollection[0].LineageID);
        }


        public override void ProcessInput(int inputID, PipelineBuffer buffer)
        {
            base.ProcessInput(inputID, buffer);
        }

        public override void PrimeOutput(int outputs, int[] outputIDs, PipelineBuffer[] buffers)
        {
            var binding = new BasicHttpBinding();
            binding.MaxReceivedMessageSize = 999999999;

            var proxy = new CAdxWebServiceXmlCCClient(binding, new EndpointAddress("http://"+Ip+":28880/adxwsvc/services/CAdxWebServiceXmlCC"));
            var parameters = new List<CAdxParamKeyValue>();
            var response = proxy.query(CallContext, PublicIdentifier, parameters.ToArray(), 2);
            
            var xml = new XmlDocument();
            xml.LoadXml(response.resultXml);

            var responseXmlWrapper = new X3ListResponseWrapper(xml);

            foreach (var line in responseXmlWrapper.Lines)
	        {
                buffers[0].AddRow();

                foreach (var field in line.Fields)
                {
                    var index = GetCollumnIndex(field.Name);

                    buffers[0].SetString(index, field.Value);
                }
	        }
            
            buffers[0].SetEndOfRowset();
        }


        private int GetCollumnIndex(string name)
        {
            var output = ComponentMetaData.OutputCollection[0];
            var columns = output.OutputColumnCollection.Cast<IDTSOutputColumn100>();
            var column = columns.Single(c => c.Name == name);

            var index = (int)BufferManager.FindColumnByLineageID(output.Buffer, column.LineageID);

            return index;
        }
        /*
        public override void SetOutputColumnDataTypeProperties(int outputID, int outputColumnID, Microsoft.SqlServer.Dts.Runtime.Wrapper.DataType dataType, int length, int precision, int scale, int codePage)
        {
            var outputColl = this.ComponentMetaData.OutputCollection;
            var output = outputColl.GetObjectByID(outputID);
            var columnColl = output.OutputColumnCollection;
            var column = columnColl.GetObjectByID(outputColumnID);

            column.SetDataTypeProperties(dataType, length, precision, scale, codePage);
        }
        */
    }
}
