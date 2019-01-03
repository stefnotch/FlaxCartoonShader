using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;
using FlaxEngine.Rendering;

namespace CartoonShader.Source.RenderingPipeline
{
	public interface IRendererDisplayer : IDisposable
	{
		IRendererInputs Inputs { get; }

		bool Enabled { get; set; }
	}
}