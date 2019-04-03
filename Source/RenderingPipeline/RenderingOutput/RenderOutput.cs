using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CartoonShader.Source.RenderingPipeline.RenderingTask;
using FlaxEngine;
using FlaxEngine.Rendering;

namespace CartoonShader.Source.RenderingPipeline.RenderingOutput
{
	public class RenderOutput
	{
		[HideInEditor]
		public IRenderer Renderer { get; set; }

		public RenderTarget RenderTarget { get; }

		public Vector2 Size
		{
			get
			{
				return RenderTarget.Size;
			}
			set
			{
				// TODO: RenderTarget also has a .Size property. Maybe I should use that?
				RenderTarget.Init(PixelFormat.R8G8B8A8_UNorm, value);
			}
		}

		public RenderOutput(Vector2 size, IRenderer renderer = null) : this(RenderTarget.New(), renderer)
		{
			Size = size;
		}

		public RenderOutput(RenderTarget renderTarget, IRenderer renderer = null)
		{
			RenderTarget = renderTarget;
			Renderer = renderer;
		}

		public RenderOutput(Texture texture)
		{
			throw new NotImplementedException();
		}
	}
}