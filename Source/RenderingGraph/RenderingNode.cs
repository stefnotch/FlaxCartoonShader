using System;
using FlaxEngine;
using NodeGraphs;

namespace RenderingGraph
{
    public abstract class RenderingNode : GraphNode
    {
        public int Index;

        [NoSerialize]
        internal RenderingGraphContext Context;
        protected int Order => Context.StartIndex + Index;

        public sealed override void OnUpdate()
        {
            base.OnUpdate();
            // Do nothing, all nodes should create a render task
        }

        public static GPUTexture CreateOutputTexture(Vector2 size)
        {
            var texture = GPUDevice.CreateTexture();
            var description = GPUTextureDescription.New2D((int)size.X, (int)size.Y, PixelFormat.R8G8B8A8_UNorm);
            texture.Init(ref description);

            return texture;
        }

        public static MaterialParameter[] GetPublicParameters(MaterialBase material)
        {
            var materialParameters = material.Parameters;
            int parameterCount = 0;
            for (int i = 0; i < materialParameters.Length; i++)
            {
                if (!materialParameters[i].IsPublic) continue;
                parameterCount++;
            }

            var publicParameters = new MaterialParameter[parameterCount];
            for (int i = 0, j = 0; i < materialParameters.Length; i++)
            {
                if (!materialParameters[i].IsPublic) continue;
                publicParameters[j] = materialParameters[i];
                j++;
            }

            return publicParameters;
        }
    }
}