using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine.Rendering;

namespace CartoonShader.Source.RenderingPipeline
{
	/// <summary>
	/// Read-only class
	/// </summary>
	public class RendererOutput
	{
		public RendererOutput(string name, RenderTarget renderTarget)
		{
			Name = name;
			RenderTarget = renderTarget;
		}

		public string Name { get; }

		public RenderTarget RenderTarget { get; }
	}
}