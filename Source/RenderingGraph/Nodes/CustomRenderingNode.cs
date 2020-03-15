using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;
using RenderingGraph;

namespace RenderingGraph.Nodes
{
    public abstract class CustomRenderingNode : RenderingNode
    {
        protected CustomRenderTask RenderTask;

        public override void OnEnable()
        {
            base.OnEnable();
            RenderTask = FlaxEngine.Object.New<CustomRenderTask>();
            RenderTask.Order = Order;
            RenderTask.Render += OnRenderUpdate;
        }

        public override void OnDisable()
        {
            RenderTask?.Dispose();
            FlaxEngine.Object.Destroy(ref RenderTask);
            base.OnDisable();
        }

        protected abstract void OnRenderUpdate(GPUContext context);
    }
}
