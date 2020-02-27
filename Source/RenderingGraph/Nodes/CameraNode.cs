using FlaxEngine;
using NodeGraphs;

namespace RenderingGraph.Nodes
{
    /// <summary>
    /// Renders a camera or the main camera
    /// </summary>
    public class CameraNode : RenderingNode<SceneRenderTask>
    {
        protected GPUTexture Output;
        protected Vector2 Size => Vector2.Max(GetInputOrDefault<Vector2>(0, Context.Size), Vector2.One);

        public CameraNode(GraphNodeDefinition definition) : base(definition)
        {
        }

        protected Camera Camera => GetInputOrDefault(1, Camera.MainCamera);

        public override void OnEnable()
        {
            base.OnEnable();
            Output = CreateOutputTexture(Size);
            RenderTask.Camera = Camera;
            RenderTask.ActorsSource = ActorsSources.Scenes;
            RenderTask.Output = Output;
            RenderTask.Begin += OnRenderTaskBegin;
            RenderTask.End += OnRenderTaskEnd;
        }

        public void OnRenderTaskBegin(SceneRenderTask task, GPUContext context)
        {
            Output.Size = Size;
        }

        public void OnRenderTaskEnd(SceneRenderTask task, GPUContext context)
        {
            Return(0, Output);
            Return(1, RenderTask.Buffers?.DepthBuffer);
            Return(2, RenderTask.Buffers?.MotionVectors);
        }

        public override void OnDisable()
        {
            if (RenderTask)
            {
                RenderTask.Begin -= OnRenderTaskBegin;
                RenderTask.End -= OnRenderTaskEnd;
                RenderTask.Dispose();
            }

            Object.Destroy(ref RenderTask);
            Object.Destroy(ref Output);
            base.OnDisable();
        }
    }
}