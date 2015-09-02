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
    [DtsPipelineComponent(DisplayName = "ComponentXml2", ComponentType = ComponentType.Transform,
        IconResource = "ComponentTests.ComponentsXml.Resources.Icon1.ico")]
    public class ComponentXml2 : PipelineComponent
    {
        private PipelineBuffer outputBuffer;
        /*
        public override void ProvideComponentProperties()
        {
            base.ProvideComponentProperties();
            var outputCollection = ComponentMetaData.OutputCollection.New();
            outputCollection.Name = "Output";
        }

        public override IDTSCustomProperty100 SetComponentProperty(string propertyName, object propertyValue)
        {
            var outputColumnCollection = ComponentMetaData.OutputCollection[0].OutputColumnCollection.New();

            outputColumnCollection.Name = "TestColumn1";
            outputColumnCollection.SetDataTypeProperties(DataType.DT_BYTES, 8000, 0, 0, 0);

            return base.SetComponentProperty(propertyName, propertyValue);
        }

        public override void SetOutputColumnDataTypeProperties(int outputID, int outputColumnID, Microsoft.SqlServer.Dts.Runtime.Wrapper.DataType dataType, int length, int precision, int scale, int codePage)
        {

            var outputColl = this.ComponentMetaData.OutputCollection;
            var output = outputColl.GetObjectByID(outputID);
            var columnColl = output.OutputColumnCollection;
            var column = columnColl.GetObjectByID(outputColumnID);

            column.SetDataTypeProperties(dataType, length, precision, scale, codePage);

        }
         * */

        public override void ReinitializeMetaData()
        {
            base.ReinitializeMetaData();

            bool hasItem = false;
            foreach (IDTSCustomProperty100 item in ComponentMetaData.CustomPropertyCollection)
	        {
                if (item.Name == "Cust1")
                    hasItem = true;
	        }

            if (!hasItem)
            {
                var outputCol = ComponentMetaData.CustomPropertyCollection.New();
                outputCol.Name = "Cust1";
                outputCol.Description = "Cust1";
                outputCol.Value = "";
            }
        }

        public override void ProvideComponentProperties()
        {
            base.ProvideComponentProperties();
            var outputCollection = ComponentMetaData.OutputCollection[0];
            outputCollection.Name = "Test Output";
            var columnCollection = outputCollection.OutputColumnCollection;
            var newColumnOutput = columnCollection.New();
            newColumnOutput.Name = "SerializedObject";
            newColumnOutput.SetDataTypeProperties(DataType.DT_NTEXT, 0, 0, 0, 0);
            outputCollection.SynchronousInputID = 0;
        }

        public override DTSValidationStatus Validate()
        {
            var validationStatus = base.Validate();
            var isOk = true;
            if (!isOk)
                validationStatus = DTSValidationStatus.VS_NEEDSNEWMETADATA;

            return validationStatus;
        }
        public override void PerformUpgrade(int pipelineVersion)
        {
            base.PerformUpgrade(pipelineVersion);
        }

        public override IDTSOutputColumn100 InsertOutputColumnAt(int outputID, int outputColumnIndex, string name, string description)
        {
            return base.InsertOutputColumnAt(outputID, outputColumnIndex, name, description);
        }
        public override void ProcessInput(int inputID, PipelineBuffer inputBuffer)
        {
            base.ProcessInput(inputID, inputBuffer);
            while(inputBuffer.NextRow())
            {
                var inputString = inputBuffer.GetString(0);

                var order = new Order() 
                { 
                    OrderCode = inputString, 
                    Product = new Product() { Name = inputString + "Product" } 
                };

                var outputXml = XmlSerializer.XmlSerialize(order);
                var outputBytes = Encoding.UTF8.GetBytes(outputXml);

                outputBuffer.AddRow();
                outputBuffer.AddBlobData(0, outputBytes);
            }
            if (inputBuffer.EndOfRowset)
                outputBuffer.SetEndOfRowset();
        }

        public override void PrimeOutput(int outputs, int[] outputIDs, PipelineBuffer[] buffers)
        {
            if (buffers.Length != 0)
                outputBuffer = buffers[0];
        }
    }
}
