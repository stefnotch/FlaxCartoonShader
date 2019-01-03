using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine.Rendering;

namespace CartoonShader.Source.RenderingPipeline
{
	public static class IRendererDisplayerExtensions
	{
		public static T SetInput<T>(this T rendererDisplayer, string name, RenderOutput renderTarget) where T : IRendererDisplayer
		{
			rendererDisplayer.Inputs[name] = renderTarget;
			return rendererDisplayer;
		}
	}
}