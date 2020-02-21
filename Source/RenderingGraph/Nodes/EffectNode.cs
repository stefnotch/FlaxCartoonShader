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
        [NoSerialize]
        private Vector2 _cachedSize;

        protected Vector2 Size => GetInputOrDefault<Vector2>(0, InputTexture?.Size ?? Vector2.One);

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
            Output = GPUDevice.CreateTexture();
            var size = Size;
            _cachedSize = size;
            var description = GPUTextureDescription.New2D((int)size.X, (int)size.Y, PixelFormat.R8G8B8A8_UNorm);
            Output.Init(ref description);
        }

        public override void OnDisable()
        {
            base.OnDisable();
            FlaxEngine.Object.Destroy(ref Output);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            var size = Size;
            if (size != _cachedSize)
            {
                Output.Size = size;
                _cachedSize = size;
            }
        }
    }
}
