using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;
using FlaxEngine.Rendering;

namespace CartoonShader.Source.RenderPipeline
{
	public class Renderer : Script
	{
		public string Name;

		public readonly List<Renderer> RenderInputs = new List<Renderer>();

		public MaterialBase Material;

		public RenderTarget RenderOutput { get => _renderTargetPostFx.Output; }

		[HideInEditor]
		[NoSerialize]
		private RenderTargetPostFx _renderTargetPostFx;

		public bool QuadForEachPixel = false;

		/*public void OnEnable()
		{
			Initialize(Screen.Size);
		}*/

		public void Initialize(Vector2 size, int order = -100000)
		{
			_renderTargetPostFx = new RenderTargetPostFx();

			foreach (var renderInput in RenderInputs)
			{
				renderInput.Initialize(size, _renderTargetPostFx.Order - 1);

				_renderTargetPostFx.RenderTargetToMaterial.Inputs.Add(renderInput.Name, renderInput.RenderOutput);
			}
			_renderTargetPostFx.QuadForEachPixel = QuadForEachPixel;
			_renderTargetPostFx.Order = order;
			_renderTargetPostFx.RenderTargetToMaterial.RenderMaterial = Material;
			_renderTargetPostFx.Initialize(size);

			ScriptUtils.Instance.AddSingleUpdate(() =>
			{
				_renderTargetPostFx.StartRenderTask();
			});
		}

		private void OnDisable()
		{
			_renderTargetPostFx.Dispose();
		}
	}
}