using Microsoft.SqlServer.Dts.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentTests.ComponentsXml
{
    [DtsPipelineComponent(DisplayName = "ComponentXml1", ComponentType = ComponentType.Transform,
        IconResource = "ComponentTests.ComponentsXml.Resources.Icon1.ico")]
    public class ComponentXml1 : XmlPipelineComponent<Order, Product>
    {
        public override void PerformUpgrade(int pipelineVersion)
        {
            base.PerformUpgrade(pipelineVersion);
        }

        public override Product ProcessInput(Order sourceObject)
        {
            var product = sourceObject.Product;

            product.Name += "Modyfied";

            return product;
        }
    }
}
