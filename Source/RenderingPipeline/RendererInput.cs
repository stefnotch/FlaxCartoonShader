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

		public RendererOutput RendererOutput; //TODO: Either a RendererInput is tied to a parent, or it has an event, or I'm totally getting rid of it
	}
}