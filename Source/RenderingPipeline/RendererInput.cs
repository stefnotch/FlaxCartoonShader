using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartoonShader.Source.RenderingPipeline
{
	public class RendererInput
	{
		public RendererInput(string name)
		{
			Name = name;
		}

		public string Name { get; }

		public RendererOutput RendererOutput;
	}
}