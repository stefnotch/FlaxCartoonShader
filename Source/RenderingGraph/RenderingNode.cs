using System;
using FlaxEngine;
using NodeGraphs;

namespace RenderingGraph
{
    public class RenderingNode : GraphNode<RenderingGraphContext>
    {
        public int NodeIndex;

        protected int Order => -Context.NodesCount + NodeIndex - 1;

        public RenderingNode(NodeDefinition definition) : base(definition)
        {
        }
        
        // TODO: Properly serialize guids instead of relying on this hack!
        public static Guid ParseGuid(object guid)
        {
            if (guid is Guid guidStruct) return guidStruct;
            if (guid == null) return Guid.Empty;
            if (guid is string guidString)
            {
                return guidString == "" ? Guid.Empty : Guid.Parse(guidString);
            }

            return Guid.Empty;
        }

        public static GPUTexture CreateOutputTexture(Vector2 size)
        {
            var texture = GPUDevice.CreateTexture();
            var description = GPUTextureDescription.New2D((int)size.X, (int)size.Y, PixelFormat.R8G8B8A8_UNorm);
            texture.Init(ref description);

            return texture;
        }
    }
}