using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;
using NodeGraphs;
using RenderingGraph.Nodes;

namespace RenderingGraph
{
    public class RenderingGraph : NodeGraph<RenderingGraphContext>
    {
        private MainNode _outputNode;
        private CustomRenderTask _renderTask;
        private float _previousTime;

        public Vector2 Size = Vector2.One;

        [NoSerialize]
        public GPUTexture Output { get; private set; }

        protected override void OnContextInitialize(RenderingGraphContext context)
        {
            base.OnContextInitialize(context);
            Context.Size = Size;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _outputNode = Nodes?.OfType<MainNode>().FirstOrDefault();
            _previousTime = Time.GameTime;

            _renderTask = FlaxEngine.Object.New<CustomRenderTask>();
            _renderTask.Render += RenderUpdate;
            _renderTask.Enabled = true;
        }

        public override void Update(float deltaTime)
        {
            // Do not execute the update loop as usual.
            // Instead, the rendering task takes care of updating
        }

        public void RenderUpdate(GPUContext context)
        {
            Context.Size = Size;
            Context.GPUContext = context;
            Context.RenderTask = _renderTask;

            float time = Time.GameTime;
            base.Update(time - _previousTime);
            _previousTime = time;

            Output = _outputNode?.GetInputOrDefault<GPUTexture>(0, null);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _renderTask?.Dispose();
            FlaxEngine.Object.Destroy(ref _renderTask);
        }
    }
}
