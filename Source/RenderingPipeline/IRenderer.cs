using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;

namespace CartoonShader.Source.RenderingPipeline
{
	// TODO: Think up better names
	public interface IRenderer : IRendererOutput, IDisposable
	{
		IRendererOutputs Outputs { get; }

		IRendererInputs Inputs { get; }

		Vector2 Size { get; set; }

		bool Enabled { get; set; }

		// TODO: PostFX
	}
}