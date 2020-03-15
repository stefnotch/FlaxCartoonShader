using System;
using FlaxEngine;
using NodeGraphs;

namespace RenderingGraph.Nodes
{
    public class TextureNode : CustomRenderingNode
    {
        protected GPUTexture Output;
        protected Vector2 Size => Vector2.Max(GetInputOrDefault<Vector2>(0, Context.Size), Vector2.One);

        public Texture Texture;

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
            context.Draw(Output, Texture);
            Return(0, Output);
        }
    }
}