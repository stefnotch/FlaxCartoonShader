using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine.Rendering;

namespace CartoonShader.Source.RenderingPipeline
{
	public interface IRendererOutput// : IObservable<IRendererOutput>
	{
		//TODO: Do I really need an event?
		RenderTarget RenderTarget { get; }

		event Action<IRendererOutput> RenderTargetChanged;
	}

	public class RendererOutput : IRendererOutput
	{
		private RenderTarget _renderTarget;

		public RendererOutput()
		{
		}

		public RenderTarget RenderTarget
		{
			get => _renderTarget;

			// TODO: Stop exposing the setter? (Accept an IObservable/event in the constructor?)
			set
			{
				_renderTarget = value;
				RenderTargetChanged?.Invoke(this);
			}
		}

		public event Action<IRendererOutput> RenderTargetChanged;
	}
}