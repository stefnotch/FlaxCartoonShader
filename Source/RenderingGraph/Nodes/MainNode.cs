using FlaxEngine;

namespace RenderingGraph.Nodes
{
    /// <summary>
    /// The main output node of the graph
    /// </summary>
    public class MainNode : CustomRenderingNode
    {
        [NoSerialize]
        public GPUTexture Output;

        public override void OnEnable()
        {
            base.OnEnable();
            Output = CreateOutputTexture(Context.Size);
        }

        protected override void OnRenderUpdate(GPUContext context)
        {
            var input = GetInputOrDefault<GPUTexture>(0, null);
            if (input)
            {
                context.Draw(Output, input);
            }
            else
            {
                context.Clear(Output, Color.Black);
            }
        }

        public override void OnDisable()
        {
            FlaxEngine.Object.Destroy(ref Output);
            base.OnDisable();
        }
    }
}
