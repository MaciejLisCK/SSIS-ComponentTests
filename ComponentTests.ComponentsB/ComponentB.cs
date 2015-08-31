using Microsoft.SqlServer.Dts.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentTests.ComponentsB
{
    [DtsPipelineComponent(DisplayName = "ComponentB", ComponentType = ComponentType.Transform,
        IconResource = "ComponentTests.ComponentsB.Resources.Icon1.ico")]
    public class ComponentB: PipelineComponent
    {
        public override void ProcessInput(int inputID, PipelineBuffer buffer)
        {
            base.ProcessInput(inputID, buffer);

            while(buffer.NextRow())
            {
                var value = buffer.GetString(0);
                buffer.SetString(0, value.ToLower());
            }
        }
    }
}
