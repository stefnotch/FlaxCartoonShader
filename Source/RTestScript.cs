using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CartoonShader.Source.RenderingPipeline;
using CartoonShader.Source.RenderingPipeline.RenderDisplayer;
using CartoonShader.Source.RenderingPipeline.Renderers;
using FlaxEngine;

namespace CartoonShader
{
	public class RTestScript : Script
	{
		public Camera Camera;
		public Material Material;
		public Material FxMaterial;

		private RenderToMaterial r;
		private SceneRenderer sr;
		private PostFxRenderer pfxr;

		private RenderPipeline renderPipeline;

		public void OnEnable()
		{
			renderPipeline = new RenderPipeline();
			renderPipeline.DefaultSize = new Vector2(100);
			var sceneRenderer = renderPipeline
				.AddRenderer(new SceneRenderer()
				{
					SourceCamera = Camera
				});
			var postFxRenderer = renderPipeline
				.AddRenderer(new PostFxRenderer()
				{
					Material = FxMaterial
				})
				.SetInput("Image", sceneRenderer.DefaultOutput);
			renderPipeline
				.AddRendererDisplayer(new RenderToMaterial()
				{
					Material = Material
				})
				.SetInput("Default", postFxRenderer.DefaultOutput); // Chaining

			renderPipeline.Enabled = true;
		}

		private void OnDisable()
		{
			renderPipeline?.Dispose();
		}
	}
}