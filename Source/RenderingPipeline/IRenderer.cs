using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;
using FlaxEngine.Rendering;

namespace CartoonShader.Source.RenderingPipeline
{
	// TODO: Think up better names
	public interface IRenderer : IRendererDisplayer
	{
		string Name { get; set; }

		RenderOutput DefaultOutput { get; }

		Vector2 Size { get; set; }

		// TODO: PostFX
	}
}