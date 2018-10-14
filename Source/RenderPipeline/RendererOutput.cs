using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;

namespace CartoonShader.Source.RenderPipeline
{
	//TODO: ExecuteInEditor (without blowing up)
	public class RendererOutput : Script
	{
		public ModelActor OutputModel;

		public readonly List<Renderer> RenderInputs = new List<Renderer>();

		public MaterialBase Material;

		[HideInEditor]
		[NoSerialize]
		private RenderTargetToMaterial _renderTargetToMaterial;

		public Vector2 Size;

		public bool UseScreenSize = false;

		public void Start()
		{
			_renderTargetToMaterial = new RenderTargetToMaterial();
			foreach (var renderInput in RenderInputs)
			{
				renderInput.Initialize(UseScreenSize ? Screen.Size : Size);
				_renderTargetToMaterial.Inputs.Add(renderInput.Name, renderInput.RenderOutput);
			}

			OutputModel.Model.WaitForLoaded();

			_renderTargetToMaterial.RenderMaterial = Material;
			_renderTargetToMaterial.ModelActor = OutputModel;
			_renderTargetToMaterial.Initialize();

			ScriptUtils.Instance.AddSingleUpdate(() =>
			{
				_renderTargetToMaterial.StartRenderTask();
			});
		}

		private void OnDisable()
		{
			_renderTargetToMaterial.Dispose();
		}
	}
}