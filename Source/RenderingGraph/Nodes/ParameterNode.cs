using FlaxEngine;
using NodeGraphs;

namespace RenderingGraph.Nodes
{
    public class ParameterNode : RenderingNode<CustomRenderTask>
    {
        public ParameterNode(GraphNodeDefinition definition) : base(definition)
        {
        }

        public override void OnEnable()
        {
            base.OnEnable();
            RenderTask.Render += OnRenderUpdate;
        }

        protected void OnRenderUpdate(GPUContext context)
        {
            base.OnUpdate();
            Return(0, GetInput(0));
        }
    }
}