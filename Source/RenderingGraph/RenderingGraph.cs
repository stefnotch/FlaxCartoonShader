using System.Linq;
using FlaxEngine;
using FlaxEngine.Utilities;
using NodeGraphs;
using RenderingGraph.Nodes;

namespace RenderingGraph
{
    public class RenderingGraph : NodeGraph<RenderingGraphContext>
    {
        private int _nodeExecutionIndex;
        private MainNode _outputNode;
        private CustomRenderTask _renderTask;

        public Vector2 Size = Vector2.One;

        [NoSerialize]
        public GPUTexture Output { get; private set; }

        protected override RenderingGraphContext CreateContext(object[] variables)
        {
            return new RenderingGraphContext()
            {
                Variables = variables,
                Size = Size,
                ExecutePreviousNodes = ExecutePreviousNodes,
                NodesCount = Nodes?.Length ?? 0
            };
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            _outputNode = Nodes?.OfType<MainNode>().FirstOrDefault();

            // This is the last render task that will get called
            // It happens just before the MainRenderTask
            _renderTask = Object.New<CustomRenderTask>();
            _renderTask.Order = -Nodes.Length - 2;
            _renderTask.Render += RenderUpdate;
            _renderTask.Enabled = true;
        }

        public override void Update(float deltaTime)
        {
            // Do not call base.Update
            if (Nodes == null || Nodes.Length <= 0) return;

            // Update the parameters
            // Each parameter will write its Value to the context
            for (int i = 0; i < Parameters.Length; i++) Parameters[i].Update(Context);
        }

        protected void ExecutePreviousNodes(int index)
        {
            for (int i = _nodeExecutionIndex; i < index; i++) Nodes[i].OnUpdate();
        }

        public void RenderUpdate(GPUContext context)
        {
            Context.Size = Size;
            Context.GPUContext = context;

            if (_outputNode != null)
            {
                ExecutePreviousNodes(_outputNode.NodeIndex);
            }
            Output = _outputNode?.GetInputOrDefault<GPUTexture>(0, null);

            // Reset
            _nodeExecutionIndex = 0;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _outputNode = null;
            if (_renderTask)
            {
                _renderTask.Render -= RenderUpdate;
                _renderTask.Dispose();
            }
            FlaxEngine.Object.Destroy(ref _renderTask);
        }
    }
}