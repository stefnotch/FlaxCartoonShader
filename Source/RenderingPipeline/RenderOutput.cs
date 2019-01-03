using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine.Rendering;

namespace CartoonShader.Source.RenderingPipeline
{
	// Not much of a choice, the other option would have been even worse
	/// <summary>
	/// Warning: You have to dispose of the underlying <see cref="RenderTarget"/> yourself
	/// </summary>
	public class RenderOutput
	{
		public readonly RenderTarget RenderTarget;
		public readonly IRenderer Renderer;

		public RenderOutput(RenderTarget renderTarget, IRenderer renderer)
		{
			RenderTarget = renderTarget;
			Renderer = renderer;
		}
	}
}