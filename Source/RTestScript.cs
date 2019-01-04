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

namespace CartoonShader
{
	[ExecuteInEditMode]
	public class RTestScript : Script
	{
		public Camera Camera;
		public Material Material;
		public Material FxMaterial;

		public UIControl RenderPipelineSurface;

		[Serialize]
		private RenderPipeline renderPipeline;

		public void OnEnable()
		{
			if (renderPipeline != null)
			{
				renderPipeline.Dispose();
				renderPipeline = null;
			}
			renderPipeline = new RenderPipeline
			{
				DefaultSize = new Vector2(100)
			};

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

			//renderPipeline.Enabled = true;

			var surface = RenderPipelineSurface.Get<RenderPipelineSurface>();
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