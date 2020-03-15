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
    /// A effect node that takes some inputs and outputs a result
    /// </summary>
    public abstract class EffectNode : CustomRenderingNode
    {
        protected GPUTexture Output;

        protected Vector2 Size => Vector2.Max(GetInputOrDefault<Vector2>(0, InputTexture?.Size ?? Vector2.One), Vector2.One);
        protected GPUTexture InputTexture => GetInputOrDefault<GPUTexture>(1, null);

        public override void OnEnable()
        {
            base.OnEnable();
            Output = CreateOutputTexture(Size);
        }

        public override void OnDisable()
        {
            FlaxEngine.Object.Destroy(ref Output);
            base.OnDisable();
        }

        protected override void OnRenderUpdate(GPUContext context)
        {
            Output.Size = Size;
        }
    }
}
