using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;
using NodeGraphs;
using RenderingGraph;

namespace RenderingGraph.Nodes
{
    /// <summary>
    /// A simple renderer node that only has outputs
    /// </summary>
    public abstract class OutputNode : RenderingNode
    {
        protected Vector2 Size => Vector2.Max(GetInputOrDefault<Vector2>(0, Context.Size), Vector2.One);

        [NoSerialize]
        public GPUTexture Output;
        
        protected OutputNode(NodeDefinition definition) : base(definition)
        {
        }

        public override void OnEnable()
        {
            base.OnEnable();
            Output = CreateOutputTexture(Size);
        }

        public override void OnDisable()
        {
            base.OnDisable();
            FlaxEngine.Object.Destroy(ref Output);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            Output.Size = Size;
        }
    }
}
