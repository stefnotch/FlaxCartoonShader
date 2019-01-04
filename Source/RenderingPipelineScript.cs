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

		public MaterialBase OutputMaterial;

		private MaterialInstance _outputMaterialInstance;

		/*
		private SceneRenderer _sceneRenderer;
		private PostFxRenderer _blurHorizontalRenderer;
		private PostFxRenderer _blurVerticalRenderer;

		private RenderToMaterial _renderToMaterial;
		*/
		private RenderPipeline _renderPipeline;
		public UIControl RenderPipelineSurface;

		private void OnEnable()
		{
			_renderPipeline = new RenderPipeline();
			_renderPipeline.DefaultSize = Screen.Size;
			var sceneRenderer = _renderPipeline
				.AddRenderer(new SceneRenderer()
				{
					SourceCamera = SceneCamera
				});

			var blurHorizontalRenderer = _renderPipeline
				.AddRenderer(new PostFxRenderer()
				{
					Material = BlurHorizontal
				})
				.SetInput("Image", sceneRenderer.DefaultOutput);

			var blurVerticalRenderer = _renderPipeline
				.AddRenderer(new PostFxRenderer()
				{
					Material = BlurHorizontal
				})
				.SetInput("Image", blurHorizontalRenderer.DefaultOutput);

			OutputMaterial.WaitForLoaded();
			_outputMaterialInstance = OutputMaterial.CreateVirtualInstance();
			(Actor as StaticModel).Entries[0].Material = _outputMaterialInstance;

			var renderToMaterial = _renderPipeline
				.AddRendererDisplayer(new RenderToMaterial()
				{
					Material = _outputMaterialInstance
				})
				.SetInput("Image", blurVerticalRenderer.DefaultOutput);

			_renderPipeline.Enabled = true;

			var surface = RenderPipelineSurface?.Get<RenderPipelineSurface>();
			if (surface != null)
			{
				surface.RenderPipeline = _renderPipeline;
			}
			/*
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
			RendererSetup();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
*/
		}

		/*
		private async Task RendererSetup()
		{
			await ActionRunner.Instance.FirstUpdate();

			_sceneRenderer = new SceneRenderer();
			_sceneRenderer.SourceCamera = SceneCamera;
			_sceneRenderer.Order = -1500000;
			_sceneRenderer.Size = Screen.Size;

			_blurHorizontalRenderer = new PostFxRenderer();
			_blurHorizontalRenderer.Material = BlurHorizontal;
			_blurHorizontalRenderer.Order = _sceneRenderer.Order + 1;
			_blurHorizontalRenderer.Size = Screen.Size;
			_blurHorizontalRenderer.Inputs["Image"] = _sceneRenderer.DefaultOutput;//.Outputs[SceneRenderer.MotionVectors];

			_blurVerticalRenderer = new PostFxRenderer();
			_blurVerticalRenderer.Material = BlurVertical;
			_blurVerticalRenderer.Order = _blurHorizontalRenderer.Order + 1;
			_blurVerticalRenderer.Size = Screen.Size;
			_blurVerticalRenderer.Inputs["Image"] = _blurHorizontalRenderer.DefaultOutput;

			OutputMaterial.WaitForLoaded();
			_outputMaterialInstance = OutputMaterial.CreateVirtualInstance();
			(Actor as StaticModel).Entries[0].Material = _outputMaterialInstance;

			_renderToMaterial = new RenderToMaterial();
			_renderToMaterial.Material = _outputMaterialInstance;
			//_renderToMaterial.Size = Screen.Size;
			_renderToMaterial.Inputs["Image"] = _blurVerticalRenderer.DefaultOutput;

			_sceneRenderer.Enabled = true;
			_blurHorizontalRenderer.Enabled = true;
			_blurVerticalRenderer.Enabled = true;
			_renderToMaterial.Enabled = true;
		}
		*/

		private void OnDisable()
		{
			/*
			_sceneRenderer?.Dispose();
			_blurHorizontalRenderer?.Dispose();
			_blurVerticalRenderer?.Dispose();
			_renderToMaterial?.Dispose();
			*/
			_renderPipeline?.Dispose();
			_renderPipeline = null;
			(Actor as StaticModel).Entries[0].Material = null;
			Destroy(ref _outputMaterialInstance);
		}
	}
}