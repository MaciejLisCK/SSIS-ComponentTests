using Microsoft.SqlServer.Dts.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentTests.ComponentsXml
{
    [DtsPipelineComponent(DisplayName = "RowsToObjectComponent1", ComponentType = ComponentType.Transform,
        IconResource = "ComponentTests.ComponentsXml.Resources.Icon1.ico")]
    public class RowsToObjectComponent1 : RowsToObjectsComponent<Order>
    {
        public override Order ProcessInput(PipelineBuffer buffer)
        {
            var result = new Order();

            var firstColumnValue = buffer.GetString(0);

            result.OrderCode = firstColumnValue;

            return result;
        }
    }
}
