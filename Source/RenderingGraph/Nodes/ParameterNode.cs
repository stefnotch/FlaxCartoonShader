namespace RenderingGraph.Nodes
{
    public class ParameterNode : RenderingNode
    {
        public ParameterNode(NodeDefinition definition) : base(definition)
        {
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            Return(0, GetInput(0));
        }
    }
}