using System.Linq;
using FlaxEngine;
using FlaxEngine.Utilities;
using NodeGraphs;
using RenderingGraph.Nodes;

namespace RenderingGraph
{
    public class RenderingGraph : NodeGraph<RenderingGraphContext>
    {
        private MainNode _outputNode;
        private CustomRenderTask _firstRenderTask;

        public Vector2 Size = Vector2.One;

        [NoSerialize]
        public GPUTexture Output { get; private set; }

        protected override RenderingGraphContext CreateContext(object[] variables)
        {
            return new RenderingGraphContext()
            {
                Variables = variables,
                Size = Size,
                StartIndex = (-Nodes?.Length ?? 0) - 1 // 0 = MainRenderTask
            };
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            _outputNode = Nodes?.OfType<MainNode>().FirstOrDefault();
            Output = _outputNode?.Output;

            // The first render task, happens before everything else
            _firstRenderTask = Object.New<CustomRenderTask>();
            _firstRenderTask.Order = Context.StartIndex - 1;
            _firstRenderTask.Render += OnRenderUpdate;
        }

        public override void Update(float deltaTime)
        {
            // Do not call base.Update
            if (Nodes == null || Nodes.Length <= 0) return;
        }

        public void OnRenderUpdate(GPUContext context)
        {
            // Update the parameters
            // Each parameter will write its Value to the context
            for (int i = 0; i < Parameters.Length; i++) Parameters[i].Update(Context);

            // Set the context data
            Context.Size = Size;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _outputNode = null;

            if (_firstRenderTask)
            {
                _firstRenderTask.Render -= OnRenderUpdate;
                _firstRenderTask.Dispose();
            }
            FlaxEngine.Object.Destroy(ref _firstRenderTask);
        }
    }
}