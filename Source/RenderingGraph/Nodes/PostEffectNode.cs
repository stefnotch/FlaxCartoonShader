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
        private MaterialInstance _materialInstance;

        public MaterialBase Material;

        public override void OnEnable()
        {
            base.OnEnable();
            if (!Material || !Material.IsPostFx) return;
            Material.WaitForLoaded();
            _materialInstance = Material.CreateVirtualInstance();

            _inputParameters = GetPublicParameters(_materialInstance);
        }

        public override void OnDisable()
        {
            FlaxEngine.Object.Destroy(ref _materialInstance);
            base.OnDisable();
        }

        protected override void OnRenderUpdate(GPUContext context)
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