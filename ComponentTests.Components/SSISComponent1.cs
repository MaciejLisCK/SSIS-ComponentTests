using Microsoft.SqlServer.Dts.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentTests.Components
{
    [DtsPipelineComponent(DisplayName = "SSISComponent1", ComponentType = ComponentType.Transform,
        IconResource = "ComponentTests.Components.Resources.Icon1.ico")]
    public class SSISComponent1 : PipelineComponent
    {
        public override void ProvideComponentProperties()
        {
            base.ProvideComponentProperties();
        }

        public override void ProcessInput(int inputID, PipelineBuffer buffer)
        {
            while (buffer.NextRow())
            {

            }
        }
    }
}
