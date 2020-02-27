using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;
using NodeGraphs;

namespace RenderingGraph.Nodes
{
    /// <summary>
    /// The main output node of the graph
    /// </summary>
    public class MainNode : RenderingNode<CustomRenderTask>
    {
        [NoSerialize]
        public GPUTexture Output;

        public MainNode(GraphNodeDefinition definition) : base(definition)
        {
        }

        public override void OnEnable()
        {
            base.OnEnable();
            RenderTask.Render += OnRenderUpdate;
            Output = CreateOutputTexture(Context.Size);
        }

        private void OnRenderUpdate(GPUContext context)
        {
            var input = GetInputOrDefault<GPUTexture>(0, null);
            if (input != null)
            {
                context.Draw(Output, input);
            }
        }

        public override void OnDisable()
        {
            base.OnDisable();
            FlaxEngine.Object.Destroy(ref Output);
        }
    }
}
