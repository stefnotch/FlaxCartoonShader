using FlaxEngine;

namespace RenderingGraph.Nodes
{
    /// <summary>
    /// An effect node
    /// </summary>
    public class PostEffectNode : EffectNode
    {
        private MaterialParameter[] _inputParameters;
        private MaterialInstance _materialInstance;

        public PostEffectNode(NodeDefinition definition) : base(definition)
        {
        }

        public override void OnEnable()
        {
            base.OnEnable();
            var material = Content.Load<MaterialBase>(ParseGuid(Definition.Values[0]));
            if (!material || !material.IsPostFx) return;
            _materialInstance = material.CreateVirtualInstance();

            var instanceParameters = _materialInstance.Parameters;
            int parameterCount = 0;
            for (int i = 0; i < instanceParameters.Length; i++)
            {
                if (!instanceParameters[i].IsPublic) continue;
                parameterCount++;
            }

            _inputParameters = new MaterialParameter[parameterCount];
            for (int i = 0, j = 0; i < instanceParameters.Length; i++)
            {
                if (!instanceParameters[i].IsPublic) continue;
                _inputParameters[j] = instanceParameters[i];
                j++;
            }
        }

        public override void OnDisable()
        {
            Object.Destroy(ref _materialInstance);
            base.OnDisable();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            var inputTexture = InputTexture;
            if (_materialInstance)
            {
                for (int i = 0; i < _inputParameters.Length; i++)
                {
                    if (!HasInput(i + 2)) continue;
                    _inputParameters[i].Value = GetInput(i + 2);
                }

                Context.GPUContext.DrawPostFxMaterial(_materialInstance, Output, inputTexture);
            }
            else
            {
                Context.GPUContext.Clear(Output, Color.Zero);
            }

            Return(0, Output);
        }
    }
}