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
    [DtsPipelineComponent(DisplayName = "SynchronousComponent", ComponentType = ComponentType.Transform,
        IconResource = "ComponentTests.ComponentsXml.Resources.Icon1.ico") ]
    public class SynchronousRowsToObjectComponent : PipelineComponent
    {
        public override void ReinitializeMetaData()
        {
            base.ReinitializeMetaData();

            var newColumnName = "NewText3";

            var outputCollection = this.ComponentMetaData.OutputCollection[0];
            var outputColumns = outputCollection.OutputColumnCollection.Cast<IDTSOutputColumn100>();

            bool containColumn = outputColumns.Any(oc => oc.Name == newColumnName);
            if (!containColumn)
            {
                var outputCol = outputCollection.OutputColumnCollection.New();
                outputCol.Name = newColumnName;
                outputCol.SetDataTypeProperties(DataType.DT_WSTR, 300, 0, 0, 0);
            }
        }

        public override void ProvideComponentProperties()
        {
            base.ProvideComponentProperties();
            var outputColl = this.ComponentMetaData.OutputCollection[0];
            var outputCol = outputColl.OutputColumnCollection.New();
            outputCol.Name = "blobData  aaa";
            outputCol.SetDataTypeProperties(DataType.DT_NTEXT, 0, 0, 0, 0);
            var outputCol2 = outputColl.OutputColumnCollection.New();
            outputCol2.Name = "tesxt  aaa";
            outputCol2.SetDataTypeProperties(DataType.DT_WSTR, 200, 0, 0, 0);
        }

        public override void ProcessInput(int inputID, PipelineBuffer buffer)
        {
            base.ProcessInput(inputID, buffer);

            while (buffer.NextRow())
            {
                var firstColumn = buffer.GetString(0);
                
                buffer.SetString(1, firstColumn + " OK");
                buffer.AddBlobData(0, new byte[] { 56, 67 });
            }

        }
    }
}


