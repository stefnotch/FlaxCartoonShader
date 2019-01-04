using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CartoonShader.Source.RenderingPipeline;
using CartoonShader.Source.RenderingPipeline.RenderDisplayer;
using CartoonShader.Source.RenderingPipeline.Renderers;
using CartoonShader.Source.RenderingPipeline.Surface;
using FlaxEngine;

namespace CartoonShader.Source
{
	// TODO: When custom Visject surfaces come out, I can get rid of this script!
	public class RenderingPipelineScript : Script
	{
		public Camera SceneCamera;

		public MaterialBase BlurHorizontal;
		public MaterialBase BlurVertical;
		public MaterialBase EdgeDetection;

		public MaterialBase OutputMaterial;

		private MaterialInstance _outputMaterialInstance;

		private RenderPipeline _renderPipeline;
		public UIControl RenderPipelineSurface;

		private void OnEnable()
		{
			_renderPipeline = new RenderPipeline();
			_renderPipeline.DefaultSize = Screen.Size;
			var sceneRenderer = _renderPipeline
				.AddRenderer(SceneCamera);

			var sceneNormalsRenderer = _renderPipeline
				.AddRenderer(SceneCamera);
			sceneNormalsRenderer.SceneRenderTask.Mode = FlaxEngine.Rendering.ViewMode.Normals;

			var blurHorizontalRenderer = _renderPipeline
				.AddRenderer(BlurHorizontal)
				.SetInput("Image", sceneRenderer.MotionVectorsOutput);

			var blurVerticalRenderer = _renderPipeline
				.AddRenderer(BlurHorizontal)
				.SetInput("Image", blurHorizontalRenderer.DefaultOutput);

			var edgeDetectionRenderer = _renderPipeline
				.AddRenderer(EdgeDetection)
				.SetInput("Image", sceneNormalsRenderer.DefaultOutput);

			var edgeDetectionOutput = edgeDetectionRenderer.DefaultOutput;
			var motionVectorsOutput = blurVerticalRenderer.DefaultOutput;

			// Output to material
			OutputMaterial.WaitForLoaded();
			_outputMaterialInstance = OutputMaterial.CreateVirtualInstance();
			(Actor as StaticModel).Entries[0].Material = _outputMaterialInstance;

			var renderToMaterial = _renderPipeline
				.AddRendererDisplayer(new RenderToMaterial()
				{
					Material = _outputMaterialInstance
				})
				.SetInput("Image", sceneRenderer.DefaultOutput);

			// Enable and debug surface
			_renderPipeline.Enabled = true;

			var surface = RenderPipelineSurface?.Get<RenderPipelineSurface>();
			if (surface != null)
			{
				surface.RenderPipeline = _renderPipeline;
			}
		}

		private void OnDisable()
		{
			_renderPipeline?.Dispose();
			_renderPipeline = null;
			(Actor as StaticModel).Entries[0].Material = null;
			Destroy(ref _outputMaterialInstance);
		}
	}
}