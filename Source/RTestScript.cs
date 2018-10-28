using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CartoonShader.Source.RenderingPipeline;
using CartoonShader.Source.RenderingPipeline.Renderers;
using FlaxEngine;

namespace CartoonShader
{
	public class RTestScript : Script
	{
		public Camera Camera;
		public Material Material;

		private RenderToMaterial r;
		private SceneRenderer sr;

		public void OnEnable()
		{
			r = new RenderToMaterial();
			r.Material = Material;
			r.Size = new Vector2(100);

			sr = new SceneRenderer();
			sr.SourceCamera = Camera;
			sr.Order = -1000000;
			sr.Size = new Vector2(100);
			r.Inputs["Default"] = sr.DefaultOutput;

			sr.Enabled = true;
			r.Enabled = true;
		}

		private void OnDisable()
		{
			r?.Dispose();
			sr?.Dispose();
		}
	}
}