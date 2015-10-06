using Microsoft.SqlServer.Dts.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentTests.ComponentsXml
{
    [DtsPipelineComponent(DisplayName = "ObjectTransformationComponent1", ComponentType = ComponentType.Transform,
        IconResource = "ComponentTests.ComponentsXml.Resources.Icon1.ico")]
    public class ObjectTransformationComponent1 : ObjectsToObjectsComponent<Order, Order>
    {
        public override Order ProcessInput(Order inputObject)
        {
            inputObject.OrderCode += "ObjectTransformationComponent1";

            return inputObject;
        }
    }
}
