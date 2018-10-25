using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine.Rendering;

namespace CartoonShader.Source.RenderingPipeline
{
	public interface IRendererOutput
	{
		string Name { get; }
		RenderTarget RenderTarget { get; set; }

		event Action<IRendererOutput> RenderTargetChanged;
	}

	public class RendererOutput : IRendererOutput
	{
		private RenderTarget _renderTarget;

		public RendererOutput(string name)
		{
			Name = name;
		}

		public string Name { get; }

		public RenderTarget RenderTarget
		{
			get => _renderTarget;

			// TODO: Stop exposing the setter!
			set
			{
				_renderTarget = value;
				RenderTargetChanged?.Invoke(this);
			}
		}

		public event Action<IRendererOutput> RenderTargetChanged;
	}
}