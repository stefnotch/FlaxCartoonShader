using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;

namespace RenderingGraph.Nodes
{
    /// <summary>
    /// An effect node
    /// </summary>
    public class PostEffectNode : EffectNode
    {
        private MaterialInstance _materialInstance;
        
        public PostEffectNode(NodeDefinition definition) : base(definition)
        {
        }

        public override void OnEnable()
        {
            base.OnEnable();
            var material = Content.Load<MaterialBase>(ParseGuid(Definition.Values[0]));
            if (material && material.IsPostFx)
            {
                _materialInstance = material.CreateVirtualInstance();
            }
        }

        public override void OnDisable()
        {
            FlaxEngine.Object.Destroy(ref _materialInstance);
            base.OnDisable();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            var inputTexture = InputTexture;
            if (inputTexture && _materialInstance)
            {
                Context.GPUContext.DrawPostFxMaterial(_materialInstance, Output, inputTexture);
            }
            Return(0, Output);
        }
    }
}
