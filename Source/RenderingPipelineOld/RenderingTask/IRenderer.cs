using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CartoonShader.Source.RenderingPipeline.RenderingOutput;
using FlaxEngine.Rendering;

namespace CartoonShader.Source.RenderingPipeline.RenderingTask
{
	public interface IRenderer : IDisposable
	{
		string Name { get; set; }
		MaterialRenderInputs Inputs { get; }
		RenderTask Task { get; }
		RenderOutput Output { get; }
	}
}