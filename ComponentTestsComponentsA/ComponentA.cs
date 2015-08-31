using Microsoft.SqlServer.Dts.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentTestsComponentsA
{
    [DtsPipelineComponent(DisplayName = "ComponentA", ComponentType = ComponentType.Transform,
        IconResource = "ComponentTestsComponentsA.Resources.Icon1.ico")]
    public class ComponentA : PipelineComponent
    {
        public override void ProcessInput(int inputID, PipelineBuffer buffer)
        {
            base.ProcessInput(inputID, buffer);
        }
    }
}
