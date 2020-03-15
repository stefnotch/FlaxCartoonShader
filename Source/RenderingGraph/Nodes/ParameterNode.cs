using FlaxEngine;
using NodeGraphs;

namespace RenderingGraph.Nodes
{
    public class ParameterNode : CustomRenderingNode
    {
        protected override void OnRenderUpdate(GPUContext context)
        {
            Return(0, GetInput(0));
        }
    }
}