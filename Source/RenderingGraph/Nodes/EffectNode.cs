using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;

namespace RenderingGraph.Nodes
{
    /// <summary>
    /// A effect node that takes some inputs and outputs a result
    /// </summary>
    public abstract class EffectNode : RenderingNode
    {
        protected Vector2 Size => Vector2.Max(GetInputOrDefault<Vector2>(0, InputTexture?.Size ?? Vector2.One), Vector2.One);

        protected GPUTexture InputTexture => GetInputOrDefault<GPUTexture>(1, null);

        [NoSerialize]
        public GPUTexture Output;

        protected EffectNode(NodeDefinition definition) 
            : base(definition)
        {
        }

        public override void OnEnable()
        {
            base.OnEnable();
            Output = CreateOutputTexture(Size);
        }

        public override void OnDisable()
        {
            base.OnDisable();
            FlaxEngine.Object.Destroy(ref Output);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            Output.Size = Size;
        }
    }
}
