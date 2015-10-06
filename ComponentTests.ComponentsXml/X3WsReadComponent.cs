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
    [DtsPipelineComponent(DisplayName = "X3WsRead", ComponentType = ComponentType.Transform,
        IconResource = "ComponentTests.ComponentsXml.Resources.Icon1.ico")]
    public class X3WsReadComponent : PipelineComponent
    {
        private const string PublicIndentifierPropertyName = "Public Identifier";
        private const string MethodPropertyName = "Method"; 
        private string lastPublicIdentifierValue = String.Empty;
        private long WebServiceMaxResponse = 1000000000;
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
                var publicIdentifierProperty = customProperties.Single(cp => cp.Name == PublicIndentifierPropertyName);

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

        private string Paramether0
        {
            get
            {
                return GetParameterByName("Paramether0").Value;
            }
        }
        private string Paramether1
        {
            get
            {
                return GetParameterByName("Paramether1").Value;
            }
        }
        private string Paramether2
        {
            get
            {
                return GetParameterByName("Paramether2").Value;
            }
        }
        private string Paramether3
        {
            get
            {
                return GetParameterByName("Paramether3").Value;
            }
        }
        private string Paramether4
        {
            get
            {
                return GetParameterByName("Paramether4").Value;
            }
        }

        private IDTSCustomProperty100 GetParameterByName(string parameterName)
        {
            var customProperties = ComponentMetaData.CustomPropertyCollection.Cast<IDTSCustomProperty100>();
            var parametherProperty = customProperties.Single(cp => cp.Name == parameterName);

            return parametherProperty;
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

        public X3WsReadComponent()
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
                .ForMember(dm => dm.Name, mo => mo.MapFrom(sm => sm.Attributes["NAM"].Value))
                .ForMember(dm => dm.Fields, mo => mo.MapFrom(sm => sm.SelectNodes("FLD").Cast<XmlNode>()));

            Mapper.CreateMap<XmlNode, X3ResultGroupTag>()
                .ForMember(dm => dm.Id, mo => mo.MapFrom(sm => sm.Attributes["ID"].Value))
                .ForMember(dm => dm.Fields, mo => mo.MapFrom(sm => sm.SelectNodes("FLD").Cast<XmlNode>()));

            Mapper.CreateMap<XmlNode, X3ResultFieldTag>()
                .ForMember(dm => dm.Name, mo => mo.MapFrom(sm => sm.Attributes["NAME"].Value))
                .ForMember(dm => dm.Value, mo => mo.MapFrom(sm => sm.InnerText))
                ;
        }

        public override void ProvideComponentProperties()
        {
            base.RemoveAllInputsOutputsAndCustomProperties();
            ComponentMetaData.RuntimeConnectionCollection.RemoveAll();

            var publicIdentifierProperty = ComponentMetaData.CustomPropertyCollection.New();
            publicIdentifierProperty.Description = "Public identifier";
            publicIdentifierProperty.Name = PublicIndentifierPropertyName;
            publicIdentifierProperty.Value = lastPublicIdentifierValue;

            var ipProperty = ComponentMetaData.CustomPropertyCollection.New();
            ipProperty.Description = "IP";
            ipProperty.Name = "IP";
            ipProperty.Value = String.Empty;
            ipProperty.ExpressionType = DTSCustomPropertyExpressionType.CPET_NOTIFY;
            
            //To powinno byc generowane automatycznie
            var paramether0Property = ComponentMetaData.CustomPropertyCollection.New();
            paramether0Property.Description = "Paramether0";
            paramether0Property.Name = "Paramether0";
            paramether0Property.Value = String.Empty;
            paramether0Property.ExpressionType = DTSCustomPropertyExpressionType.CPET_NOTIFY;
            
            var paramether1Property = ComponentMetaData.CustomPropertyCollection.New();
            paramether1Property.Description = "Paramether1";
            paramether1Property.Name = "Paramether1";
            paramether1Property.Value = String.Empty;
            paramether1Property.ExpressionType = DTSCustomPropertyExpressionType.CPET_NOTIFY;
            
            var paramether2Property = ComponentMetaData.CustomPropertyCollection.New();
            paramether2Property.Description = "Paramether2";
            paramether2Property.Name = "Paramether2";
            paramether2Property.Value = String.Empty;
            paramether2Property.ExpressionType = DTSCustomPropertyExpressionType.CPET_NOTIFY;
            
            var paramether3Property = ComponentMetaData.CustomPropertyCollection.New();
            paramether3Property.Description = "Paramether3";
            paramether3Property.Name = "Paramether3";
            paramether3Property.Value = String.Empty;
            paramether3Property.ExpressionType = DTSCustomPropertyExpressionType.CPET_NOTIFY;
            
            var paramether4Property = ComponentMetaData.CustomPropertyCollection.New();
            paramether4Property.Description = "Paramether4";
            paramether4Property.Name = "Paramether4";
            paramether4Property.Value = String.Empty;
            paramether4Property.ExpressionType = DTSCustomPropertyExpressionType.CPET_NOTIFY;

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
            var publicIdentifierProperty = customProperties.Single(cp => cp.Name == PublicIndentifierPropertyName);

            if (String.IsNullOrWhiteSpace(publicIdentifierProperty.Value))
                return DTSValidationStatus.VS_ISBROKEN;

            bool hasOutput = ComponentMetaData.OutputCollection.Count > 0;
            bool isLastPublicIndentifierEmpty = String.IsNullOrWhiteSpace(lastPublicIdentifierValue);
            bool hasPublicIdentifierValueChanged = publicIdentifierProperty.Value != lastPublicIdentifierValue;
            if ((!hasOutput && hasPublicIdentifierValueChanged) || (hasOutput && hasPublicIdentifierValueChanged && !isLastPublicIndentifierEmpty) )
            {
                lastPublicIdentifierValue = publicIdentifierProperty.Value;
                return DTSValidationStatus.VS_ISVALID;
            }

            return DTSValidationStatus.VS_ISVALID;
        }

        private void GenerateInputsAndOutputs()
        {
            var binding = new BasicHttpBinding();
            binding.MaxReceivedMessageSize = WebServiceMaxResponse;

            var proxy = new CAdxWebServiceXmlCCClient(binding, new EndpointAddress("http://"+Ip+":28880/adxwsvc/services/CAdxWebServiceXmlCC"));
            var parameters = new List<CAdxParamKeyValue>();
            var response = proxy.getDescription(CallContext, PublicIdentifier);

            var descriptionRawXml = response.resultXml;
            var descriptionXml = new XmlDocument();
            descriptionXml.LoadXml(descriptionRawXml);

            var description = new X3XmlDescriptionWrapper(descriptionXml);


            if (ComponentMetaData.InputCollection.Count == 0)
            {
                var inputCollection = ComponentMetaData.InputCollection.New();
                inputCollection.Name = "Input";
                var inputCollumnsCollection = inputCollection.InputColumnCollection;
                foreach (var group in description.ReadDescription)
                {
                    foreach (var field in group.Fields)
                    {
                        var inputColumn = inputCollumnsCollection.New();

                        inputColumn.Name = group.Name + "|" + field.Name;
                    }
                }
            }

            IDTSOutput100 output;
            bool hasOutputCollection = ComponentMetaData.OutputCollection.Count > 0;
            if (!hasOutputCollection)
            {
                output = ComponentMetaData.OutputCollection.New();
                output.Name = "Output";
            }
            else
                output = ComponentMetaData.OutputCollection[0];

            var outputColumnsCollection = output.OutputColumnCollection;
            var outputColumns = outputColumnsCollection.Cast<IDTSOutputColumn100>();

            foreach (var group in description.ObjectDescription)
            {
                foreach (var field in group.Fields)
                {
                    var fieldName = group.Name + "|" + field.Name;
                    if (field.DataType == "Char")
                    {
                        IDTSOutputColumn100 newColumn;
                        if (outputColumns.Any(c => c.Name == fieldName))
                            newColumn = outputColumns.Single(c => c.Name == fieldName);
                        else
                            newColumn = outputColumnsCollection.New();

                        newColumn.Name = fieldName;
                        switch (field.DataType)
                        {
                            case "Char":
                                newColumn.SetDataTypeProperties(DataType.DT_WSTR, (int)field.Length, 0, 0, 0);
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                    }
                }
            }
        }

        public override void ReinitializeMetaData()
        {
            base.ReinitializeMetaData();

            GenerateInputsAndOutputs();
        }

        public override void ProcessInput(int inputID, PipelineBuffer inputBuffer)
        {
            while (inputBuffer.NextRow())
            {
                var inputCollection = ComponentMetaData.InputCollection[0];
                var inputColumns = inputCollection.InputColumnCollection.Cast<IDTSInputColumn100>().ToList();
                var parameters = new List<CAdxParamKeyValue>();
                for (int i = 0; i < inputColumns.Count; i++)
                {
                    var inputColumn = inputColumns[i];
                    var inputColumnIndex = GetInputCollumnIndex(inputColumn.Name);
                    var inputColumnValue = inputBuffer.GetString(inputColumnIndex);

                    parameters.Add(new CAdxParamKeyValue() { key = i.ToString(), value = inputColumnValue });
                }

                var binding = new BasicHttpBinding();
                binding.MaxReceivedMessageSize = WebServiceMaxResponse;

                var proxy = new CAdxWebServiceXmlCCClient(binding, new EndpointAddress("http://" + Ip + ":28880/adxwsvc/services/CAdxWebServiceXmlCC"));

                var response = proxy.read(CallContext, PublicIdentifier, parameters.ToArray());

                var xml = new XmlDocument();
                xml.LoadXml(response.resultXml);

                var responseXmlWrapper = new X3ObjectResponseWrapper(xml);

                outputBuffer.AddRow();
                foreach (var groupTag in responseXmlWrapper.ResultGroups)
                {
                    foreach (var field in groupTag.Fields)
                    {
                        var fieldName = groupTag.Id + "|" + field.Name;

                        try
                        {
                            var index = GetOutputCollumnIndex(fieldName);

                            outputBuffer.SetString(index, field.Value);
                        }
                        catch(Exception e) {}
                    }
                }
            }

            if (inputBuffer.EndOfRowset)
                outputBuffer.SetEndOfRowset();
        }

        PipelineBuffer outputBuffer;

        public override void PrimeOutput(int outputs, int[] outputIDs, PipelineBuffer[] buffers)
        {
            outputBuffer = buffers[0];
        }

        private int GetInputCollumnIndex(string name)
        {
            var inputCollection = ComponentMetaData.InputCollection[0];
            var columns = inputCollection.InputColumnCollection.Cast<IDTSInputColumn100>();
            var column = columns.Single(c => c.Name == name);

            var index = (int)BufferManager.FindColumnByLineageID(inputCollection.Buffer, column.LineageID);

            return index;
        }


        private int GetOutputCollumnIndex(string name)
        {
            var outputCollection = ComponentMetaData.OutputCollection[0];
            var columns = outputCollection.OutputColumnCollection.Cast<IDTSOutputColumn100>();
            var column = columns.Single(c => c.Name == name);

            var index = (int)BufferManager.FindColumnByLineageID(outputCollection.Buffer, column.LineageID);

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
