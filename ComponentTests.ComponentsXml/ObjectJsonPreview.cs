using ComponentTests.Utitlities;
using Microsoft.SqlServer.Dts.Pipeline;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace ComponentTests.ComponentsXml
{
    [DtsPipelineComponent(DisplayName = "ObjectJsonPreview", ComponentType = ComponentType.Transform,
        IconResource = "ComponentTests.ComponentsXml.Resources.Icon1.ico")]
    public class ObjectJsonPreview : PipelineComponent
    {
        private PipelineBuffer outputBuffer;
        private Type TSource = typeof(Order);

        public override void ProvideComponentProperties()
        {
            base.ProvideComponentProperties();
            var inputCollection = ComponentMetaData.InputCollection[0];
            inputCollection.Name = "Input";
            var inputColumnCollection = inputCollection.InputColumnCollection;

            var outputCollection = ComponentMetaData.OutputCollection[0];
            outputCollection.Name = "Output";
            var outputColumnCollection = outputCollection.OutputColumnCollection;
            var newColumnOutput = outputColumnCollection.New();
            newColumnOutput.Name = String.Format("[{0}] Preview", TSource.ToString());
            newColumnOutput.SetDataTypeProperties(DataType.DT_WSTR, 4000, 0, 0, 0);
            outputCollection.SynchronousInputID = 0;
        }

        public override DTSValidationStatus Validate()
        {
            var inputCollection = ComponentMetaData.InputCollection[0];
            var inputColumnCollection = inputCollection.InputColumnCollection;

            bool hasInputColumns = inputColumnCollection.Count > 0;
            if (!hasInputColumns)
            {
                FireError(@"Control sould have at least one input column.");
                return DTSValidationStatus.VS_ISBROKEN;
            }

            var firstInputColumn = inputColumnCollection[0];
            bool isFirstInputColumnTextStream = firstInputColumn.DataType == DataType.DT_NTEXT;
            if (!isFirstInputColumnTextStream)
            {
                FireError(@"First input column should be the type of ""Unicode test stream"" (DT_NTEXT).");
                return DTSValidationStatus.VS_ISBROKEN;
            }

            bool isFirstInputColumnValidType = firstInputColumn.Name == TSource.ToString();
            if (!isFirstInputColumnValidType)
            {
                FireError(@"Type of input column is not valid. Name of column should be the same as a source object type.");
                return DTSValidationStatus.VS_ISBROKEN;
            }

            return base.Validate();
        }
        public override void PrimeOutput(int outputs, int[] outputIDs, PipelineBuffer[] buffers)
        {
            if (buffers.Length != 0)
                outputBuffer = buffers[0];
        }
        public override void ProcessInput(int inputID, PipelineBuffer inputBuffer)
        {
            base.ProcessInput(inputID, inputBuffer);

            var jsSerializer = new JavaScriptSerializer();
            while (inputBuffer.NextRow())
            {
                var inputBytes = inputBuffer.GetBlobData(0, 0, (int)inputBuffer.GetBlobLength(0));
                var inputXml = Encoding.UTF8.GetString(inputBytes);
                var inputObject = XmlSerializer.XmlDeserialize<Order>(inputXml);

                var serializedObject = jsSerializer.Serialize(inputObject);
                if (serializedObject.Length > 4000 - 4)
                    serializedObject = serializedObject.Substring(0, 4000 - 4) + " ...";

                outputBuffer.AddRow();
                outputBuffer.SetString(0, serializedObject);
            }

            if (inputBuffer.EndOfRowset)
                outputBuffer.SetEndOfRowset();
        }

        private void FireError(string message)
        {
            bool cancel;
            ComponentMetaData.FireError(0, ComponentMetaData.Name, message, string.Empty, 0, out cancel);
        }
    }
}
