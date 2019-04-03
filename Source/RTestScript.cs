using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CartoonShader.Source.RenderingPipeline;
using CartoonShader.Source.RenderingPipeline.Surface;
using FlaxEngine;

namespace CartoonShader
{
	public class RTestScript : Script
	{
		public Camera Camera;
		public Material Material;
		public Material FxMaterial;

		public UIControl RenderPipelineSurface;

		private RenderPipeline renderPipeline;

		public void OnEnable()
		{
			if (renderPipeline != null)
			{
				renderPipeline.Dispose();
				renderPipeline = null;
			}
			renderPipeline = new RenderPipeline(new Vector2(100));

			var sceneRenderer = renderPipeline
				.AddCameraRenderer(Camera, "Scene");
			var postFxRenderer = renderPipeline
				.AddPostEffectRenderer(FxMaterial, "Post Processing")
				.SetInput("Image", sceneRenderer.Output);

			/*renderPipeline
				.AddRendererDisplayer(new RenderToMaterial()
				{
					Material = Material
				})
				.SetInput("Default", postFxRenderer.DefaultOutput); // Chaining
				*/
			renderPipeline.Enabled = true;

			var surface = RenderPipelineSurface?.Get<RenderPipelineSurface>();
			if (surface != null)
			{
				surface.RenderPipeline = renderPipeline;
			}
		}

		private void OnDisable()
		{
			renderPipeline?.Dispose();
			renderPipeline = null;
		}
	}
}