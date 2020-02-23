using FlaxEngine;

namespace RenderingGraph.Nodes
{
    /// <summary>
    /// Renders a camera or the main camera
    /// </summary>
    public class CameraNode : OutputNode
    {
        protected SceneRenderTask RenderTask;

        public CameraNode(NodeDefinition definition) : base(definition)
        {
        }

        protected Camera Camera => GetInputOrDefault(1, Camera.MainCamera);

        public override void OnEnable()
        {
            base.OnEnable();
            RenderTask = Object.New<SceneRenderTask>();
            RenderTask.Enabled = false;
            RenderTask.Order = Order;
            RenderTask.Camera = Camera;
            RenderTask.ActorsSource = ActorsSources.Scenes;
            RenderTask.Output = Output;
            RenderTask.Begin += OnRenderTaskBegin;
            RenderTask.End += OnRenderTaskEnd;
            RenderTask.Enabled = true;
        }

        public override void OnUpdate()
        {
            // Nothing
        }

        public void OnRenderTaskBegin(SceneRenderTask task, GPUContext context)
        {
            Context.ExecutePreviousNodes(NodeIndex);
        }

        public void OnRenderTaskEnd(SceneRenderTask task, GPUContext context)
        {
            if (!RenderTask) return;

            Return(0, Output);
            Return(1, RenderTask.Buffers.DepthBuffer);
            Return(2, RenderTask.Buffers.MotionVectors);
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
            base.OnDisable();
        }
    }
}