using ComponentTests.Utitlities;
using Microsoft.SqlServer.Dts.Pipeline;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentTests.ComponentsXml
{
    public abstract class XmlPipelineComponent<TSource, TDestination> : PipelineComponent
        where TSource : class, new()
        where TDestination : class, new()
    {
        private PipelineBuffer outputBuffer;

        public abstract TDestination ProcessInput(TSource inputObject);

        public override void ProcessInput(int inputID, PipelineBuffer inputBuffer)
        {
            base.ProcessInput(inputID, inputBuffer);
            while (inputBuffer.NextRow())
            {
                var inputBytes = inputBuffer.GetBlobData(0, 0, (int)inputBuffer.GetBlobLength(0));
                var inputXml = Encoding.UTF8.GetString(inputBytes);
                var inputObject = XmlSerializer.XmlDeserialize<TSource>(inputXml);

                var outputObject = ProcessInput(inputObject);

                var outputXml = XmlSerializer.XmlSerialize(outputObject);
                var outputBytes = Encoding.UTF8.GetBytes(outputXml);
                outputBuffer.AddRow();
                outputBuffer.AddBlobData(0, outputBytes);
            }

            if (inputBuffer.EndOfRowset)
                outputBuffer.SetEndOfRowset();
        }

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
            newColumnOutput.Name = typeof(TDestination).ToString();
            newColumnOutput.SetDataTypeProperties(DataType.DT_NTEXT, 0, 0, 0, 0);
            outputCollection.SynchronousInputID = 0;
        }

        public override DTSValidationStatus Validate()
        {
            var inputCollection = ComponentMetaData.InputCollection[0];
            var inputColumnCollection = inputCollection.InputColumnCollection;

            var firstInputColumn = inputColumnCollection[0];
            bool isFirstInputColumnTextStream = firstInputColumn.DataType == DataType.DT_NTEXT;
            if (!isFirstInputColumnTextStream)
            {
                FireError(@"First input column should be the type of ""Unicode test stream"" (DT_NTEXT).");
                return DTSValidationStatus.VS_ISBROKEN;
            }

            bool isFirstInputColumnValidType = firstInputColumn.Name == typeof(TSource).ToString();
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

        private void FireError(string message)
        {
            bool cancel;
            ComponentMetaData.FireError(0, ComponentMetaData.Name, message, string.Empty, 0, out cancel);
        }
    }
}
/*
        var inputOrderXml = Row.OrderXml.ConvertToString();
        var order = XmlSerializer.XmlDeserialize<Order>(inputOrderXml);
        var mocaProxy = new MocaProxy();

        var xmlOffer = mocaProxy.GetOffer();

        var orderSerialized = XmlSerializer.XmlSerialize<Order>(order);
        Output0Buffer.OrderXml.AddStringData(orderSerialized); 
*/