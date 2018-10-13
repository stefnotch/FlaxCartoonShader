using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;

namespace CartoonShader.Source.RenderPipeline
{
	public class RendererOutput : Script
	{
		public ModelActor OutputModel;

		public readonly List<Renderer> RenderInputs = new List<Renderer>();

		public MaterialBase Material;

		[HideInEditor]
		public readonly RenderTargetOutput RenderTargetOutput = new RenderTargetOutput();

		public Vector2 Size;

		public void Start()
		{
			foreach (var renderInput in RenderInputs)
			{
				renderInput.Initialize(Size);
				RenderTargetOutput.Inputs.Add(renderInput.Name, renderInput.RenderOutput);
			}

			OutputModel.Model.WaitForLoaded();

			RenderTargetOutput.RenderMaterial = Material;
			RenderTargetOutput.ModelActor = OutputModel;
			RenderTargetOutput.Initialize();
			RenderTargetOutput.StartRenderTask();
		}
	}
}