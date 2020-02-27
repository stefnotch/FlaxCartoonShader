using System;
using FlaxEngine;
using NodeGraphs;

namespace RenderingGraph
{
    public abstract class RenderingNode<TRenderTask> : GraphNode<RenderingGraphContext> where TRenderTask : RenderTask
    {
        public int NodeIndex;

        protected int Order => Context.StartIndex + NodeIndex;

        protected TRenderTask RenderTask;

        protected RenderingNode(GraphNodeDefinition definition) : base(definition)
        {
        }

        public override void OnEnable()
        {
            base.OnEnable();
            RenderTask = FlaxEngine.Object.New<TRenderTask>();
            RenderTask.Order = Order;
        }

        public sealed override void OnUpdate()
        {
            base.OnUpdate();
            // Do nothing, all nodes should create a render task
        }

        public override void OnDisable()
        {
            base.OnDisable();

            RenderTask?.Dispose();
            FlaxEngine.Object.Destroy(ref RenderTask);
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