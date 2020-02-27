using FlaxEngine;
using NodeGraphs;

namespace RenderingGraph.Nodes
{
    /// <summary>
    /// An effect node
    /// </summary>
    public class PostEffectNode : EffectNode
    {
        private MaterialParameter[] _inputParameters;
        private MaterialBase _material;
        private MaterialInstance _materialInstance;

        // TODO: It's probably a good idea to store a proper reference to the material
        // public MaterialBase Material;

        public PostEffectNode(GraphNodeDefinition definition) : base(definition)
        {
        }

        public override void OnEnable()
        {
            base.OnEnable();
            _material = Content.Load<MaterialBase>(ParseGuid(Definition.Values[0]));
            if (!_material || !_material.IsPostFx) return;
            _materialInstance = _material.CreateVirtualInstance();

            _inputParameters = GetPublicParameters(_materialInstance);
        }

        public override void OnDisable()
        {
            FlaxEngine.Object.Destroy(ref _materialInstance);
            FlaxEngine.Object.Destroy(ref _material);
            base.OnDisable();
        }

        public override void OnRenderUpdate(GPUContext context)
        {
            base.OnRenderUpdate(context);
            var inputTexture = InputTexture;
            if (_materialInstance)
            {
                for (int i = 0; i < _inputParameters.Length; i++)
                {
                    if (!HasInput(i + 2)) continue;
                    _inputParameters[i].Value = GetInput(i + 2);
                }

                context.DrawPostFxMaterial(_materialInstance, Output, inputTexture);
            }
            else
            {
                context.Clear(Output, Color.Zero);
            }

            Return(0, Output);
        }
    }
}