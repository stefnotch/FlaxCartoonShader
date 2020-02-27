using System;
using FlaxEngine;
using NodeGraphs;

namespace RenderingGraph.Nodes
{
    public class TextureNode : RenderingNode<CustomRenderTask>
    {
        private Texture _texture;
        protected GPUTexture Output;
        protected Vector2 Size => Vector2.Max(GetInputOrDefault<Vector2>(0, Context.Size), Vector2.One);

        public TextureNode(GraphNodeDefinition definition) : base(definition)
        {
        }

        public override void OnEnable()
        {
            base.OnEnable();
            _texture = Content.Load<Texture>(ParseGuid(Definition.Values[0]));
            Output = CreateOutputTexture(Size);
            RenderTask.Render += OnRenderUpdate;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            FlaxEngine.Object.Destroy(ref Output);
            FlaxEngine.Object.Destroy(ref _texture);
        }

        public void OnRenderUpdate(GPUContext context)
        {
            base.OnUpdate();
            Output.Size = Size;
            context.Draw(Output, _texture);
            Return(0, Output);
        }
    }
}