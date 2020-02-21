using System;
using FlaxEngine;

namespace RenderingGraph.Nodes
{
    public class TextureNode : OutputNode
    {
        private Texture _texture;
        public TextureNode(NodeDefinition definition) : base(definition)
        {
        }

        public override void OnEnable()
        {
            base.OnEnable();
            _texture = Content.Load<Texture>(ParseGuid(Definition.Values[0]));
        }

        public override void OnDisable()
        {
            base.OnDisable();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            // TODO: Only do this when the size has changed
            Context.GPUContext.Draw(Output, _texture);

            Return(0, Output);
        }
    }
}