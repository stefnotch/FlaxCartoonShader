using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;
using FlaxEngine.Rendering;

namespace CartoonShader.Source.RenderPipeline
{
	public abstract class Renderer : Script
	{
		public string Name { get; set; }
		public abstract RenderTarget RenderOutput { get; }

		public abstract void Initialize(Vector2 size, int order = -100000);
	}
}