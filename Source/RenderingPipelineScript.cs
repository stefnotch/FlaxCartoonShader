using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CartoonShader.Source.RenderingPipeline;
using CartoonShader.Source.RenderingPipeline.RenderingOutput;
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

		public StaticModel OutputMaterialActor;
		public MaterialBase OutputMaterial;

		private RenderPipeline _renderPipeline;
		public UIControl RenderPipelineSurface;

		private void OnEnable()
		{
			Scripting.InvokeOnUpdate(async () =>
			{
				_renderPipeline = new RenderPipeline(Screen.Size);

				var sceneRenderer = _renderPipeline
					.AddCameraRenderer(SceneCamera, "Scene");

				var sceneNormalsRenderer = _renderPipeline
					.AddCameraRenderer(SceneCamera, "Normal Vectors");
				sceneNormalsRenderer.Task.Mode = FlaxEngine.Rendering.ViewMode.Normals;

				var blurHorizontalRenderer = _renderPipeline
					.AddPostEffectRenderer(BlurHorizontal, "Blur Horizontal (Motion Vectors)")
					.SetInput("Image", await sceneRenderer.MotionVectorsOutput);

				var blurVerticalRenderer = _renderPipeline
					.AddPostEffectRenderer(BlurVertical, "Blur Vertical")
					.SetInput("Image", blurHorizontalRenderer.Output);

				var edgeDetectionRenderer = _renderPipeline
					.AddPostEffectRenderer(EdgeDetection, "Edge Detection")
					.SetInput("Image", sceneNormalsRenderer.Output);

				// Now we got those things to work with:
				var edgeDetectionOutput = edgeDetectionRenderer.Output;
				var motionVectorsOutput = blurVerticalRenderer.Output;

				var displacedCombination = _renderPipeline
					.AddPixelsRenderer(MotionVectorsAndSceneCombination, "Displaced Combination")
					.SetInput("Image", edgeDetectionOutput)
					.SetInput("MotionVectors", motionVectorsOutput);

				// TODO: Output displacedCombination to a cube or something

				//var displacedRenderer = _renderPipeline
				//	.AddRenderer(DisplacedCamera);

				//TODO: Output to material

				// Output the displaced combination to a material
				//var renderToMaterial = _renderPipeline
				//	.ShowRenderOutput(displacedCombination.Output, OutputMaterialActor, OutputMaterial);

				// Output the motion vectors (debug purrposes)
				var motionVectorsDebug = _renderPipeline
					.AddPostEffectRenderer(MotionVectorsDebug, "Motion Vectors") // Can't use the RendererDisplayers cause the debug view can't display their materials yet..
					.SetInput("Image", await sceneRenderer.MotionVectorsOutput);

				// Enable and debug surface
				_renderPipeline.Enabled = true;

				// I have to set the MotionVectorsOutput **after** starting the render pipeline (which starts the tasks)
				// (Because the MotionVectorsOutput only exists after the tasks have started)
				/*blurHorizontalRenderer
					.SetInput("Image", await sceneRenderer.MotionVectorsOutput);
				motionVectorsDebug
					.SetInput("Image", await sceneRenderer.MotionVectorsOutput);*/

				var surface = RenderPipelineSurface?.Get<RenderPipelineSurface>();
				if (surface != null)
				{
					surface.RenderPipeline = _renderPipeline;
				}
			});
		}

		private void OnDisable()
		{
			_renderPipeline?.Dispose();
			_renderPipeline = null;
		}
	}
}