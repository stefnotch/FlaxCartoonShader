using System.Linq;
using FlaxEngine;
using FlaxEngine.Utilities;
using NodeGraphs;
using RenderingGraph.Nodes;

namespace RenderingGraph
{
    public class RenderingGraph : NodeGraph<RenderingNode>
    {
        private MainNode _outputNode;
        private CustomRenderTask _firstRenderTask;
        private RenderingGraphContext _context;

        public Vector2 Size = Vector2.One;

        [NoSerialize]
        public GPUTexture Output => _outputNode?.Output;

        protected override void OnEnable()
        {
            _context = new RenderingGraphContext
            {
                Size = Size,
                StartIndex = (-Nodes?.Length ?? 0) - 1 // 0 = MainRenderTask
            };

            // The first render task, happens before everything else
            _firstRenderTask = Object.New<CustomRenderTask>();
            _firstRenderTask.Order = _context.StartIndex - 1;
            _firstRenderTask.Render += OnRenderUpdate;

            // Output node
            _outputNode = Nodes?.OfType<MainNode>().FirstOrDefault();

            // Set additional node data
            Nodes.ForEach(n => n.Context = _context);

            // Enable everything
            base.OnEnable();
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
            for (int i = 0; i < Parameters.Length; i++) Parameters[i].Update(Variables);

            // Set the context data
            _context.Size = Size;
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