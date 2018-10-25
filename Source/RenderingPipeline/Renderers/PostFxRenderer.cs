using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;

namespace CartoonShader.Source.RenderingPipeline.Renderers
{
	public class PostFxRenderer : RendererWithTask
	{
		public override IRendererOutput DefaultOutput => base.DefaultOutput;

		protected override void Enable(bool enabled)
		{
			base.Enable(enabled);
		}

		protected override void OrderChanged(int order)
		{
			base.OrderChanged(order);
		}

		protected override void SizeChanged(Vector2 size)
		{
			base.SizeChanged(size);
		}

		#region IDisposable Support

		private bool disposedValue = false; // To detect redundant calls

		protected override void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					//FlaxEngine.Object.Destroy(ref _task);
					//FlaxEngine.Object.Destroy(ref _defaultOutput);
				}
				disposedValue = true;
			}
			base.Dispose(disposing);
		}

		#endregion IDisposable Support
	}
}