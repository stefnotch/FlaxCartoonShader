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

		public MaterialBase MotionVectorsDebug;

		public MaterialBase MotionVectorsAndSceneCombination;
		public Camera DisplacedCamera;

		public MaterialBase OutputMaterial;

		private MaterialInstance _outputMaterialInstance;

		private RenderPipeline _renderPipeline;
		public UIControl RenderPipelineSurface;

		private void OnEnable()
		{
			_renderPipeline = new RenderPipeline();
			_renderPipeline.DefaultSize = Screen.Size;

			var sceneRenderer = _renderPipeline
				.AddRenderer(SceneCamera, "Scene");

			var sceneNormalsRenderer = _renderPipeline
				.AddRenderer(SceneCamera, "Normal Vectors");
			sceneNormalsRenderer.SceneRenderTask.Mode = FlaxEngine.Rendering.ViewMode.Normals;

			var blurHorizontalRenderer = _renderPipeline
				.AddRenderer(BlurHorizontal, "Blur Horizontal (Motion Vectors)")
				.SetInput("Image", sceneRenderer.MotionVectorsOutput);

			var blurVerticalRenderer = _renderPipeline
				.AddRenderer(BlurVertical, "Blur Vertical")
				.SetInput("Image", blurHorizontalRenderer.DefaultOutput);

			var edgeDetectionRenderer = _renderPipeline
				.AddRenderer(EdgeDetection, "Edge Detection")
				.SetInput("Image", sceneNormalsRenderer.DefaultOutput);

			// Now we got those things to work with:
			var edgeDetectionOutput = edgeDetectionRenderer.DefaultOutput;
			var motionVectorsOutput = blurVerticalRenderer.DefaultOutput;

			var displacedCombination = _renderPipeline
				.AddRendererDisplayer(new RenderToMaterial()
				{
					Material = MotionVectorsAndSceneCombination
				})
				.SetInput("Image", edgeDetectionOutput)
				.SetInput("MotionVectors", motionVectorsOutput);

			var displacedRenderer = _renderPipeline
				.AddRenderer(DisplacedCamera);

			// Output to material
			OutputMaterial.WaitForLoaded();
			_outputMaterialInstance = OutputMaterial.CreateVirtualInstance();
			(Actor as StaticModel).Entries[0].Material = _outputMaterialInstance;

			var renderToMaterial = _renderPipeline
				.AddRendererDisplayer(new RenderToMaterial()
				{
					Material = _outputMaterialInstance
				})
				.SetInput("Image", displacedRenderer.DefaultOutput);

			var motionVectorsDebug = _renderPipeline
				.AddRenderer(MotionVectorsDebug, "Motion Vectors") // Can't use the RendererDisplayers cause the debug view can't display their materials yet..
				.SetInput("Image", sceneRenderer.MotionVectorsOutput);

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