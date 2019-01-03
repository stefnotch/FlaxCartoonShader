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
			r = new RenderToMaterial();
			r.Material = Material;
			//r.Size = new Vector2(100);

			sr = new SceneRenderer();
			sr.SourceCamera = Camera;
			sr.Order = -1000000;
			sr.Size = new Vector2(100);

			pfxr = new PostFxRenderer();
			pfxr.Material = FxMaterial;
			pfxr.Order = sr.Order + 1;
			pfxr.Size = new Vector2(100);
			pfxr.Inputs["Image"] = sr.DefaultOutput;

			r.Inputs["Default"] = pfxr.DefaultOutput;

			sr.Enabled = true;
			pfxr.Enabled = true;
			r.Enabled = true;

			/*
			var renderPipeline = new RenderPipeline();
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
			// TODO: Order as well
			*/
		}

		private void OnDisable()
		{
			r?.Dispose();
			sr?.Dispose();
			pfxr?.Dispose();

			//renderPipeline?.Dispose();
		}
	}
}