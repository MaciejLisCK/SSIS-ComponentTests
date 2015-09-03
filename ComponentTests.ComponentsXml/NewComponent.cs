using Microsoft.SqlServer.Dts.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentTests.ComponentsXml
{
    [DtsPipelineComponent(DisplayName = "NewComponent", ComponentType = ComponentType.Transform,
        IconResource = "ComponentTests.ComponentsXml.Resources.Icon1.ico",
        CurrentVersion = 2)]
    public class NewComponent : XmlPipelineComponent<Order, Order>
    {
        public override Order ProcessInput(Order sourceObject)
        {
            return sourceObject;
        }
    }
}
