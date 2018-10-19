using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;

namespace CartoonShader.Source.RenderingPipeline.Renderers
{
	public class SceneRenderer : RendererWithTask
	{
		/// <summary>
		/// Camera source
		/// </summary>
		public Camera SourceCamera;

		private const string DepthBuffer = "DepthBuffer";
		private const string MotionVectors = "MotionVectors";

		protected override void Enable(bool enabled)
		{
			base.Enable(enabled);
			if (enabled)
			{
				_task.Camera = SourceCamera;
				AddOutput(new RendererOutput(DepthBuffer, null));
				AddOutput(new RendererOutput(DepthBuffer, null));
			}
		}

		protected override void EnableRenderTask(bool enabled)
		{
			base.EnableRenderTask(enabled);
			if (enabled)
			{
				AddOutput(new RendererOutput(DepthBuffer, _task.Buffers.DepthBuffer));
				AddOutput(new RendererOutput(MotionVectors, _task.Buffers.MotionVectors));
			}
		}

		protected override void MaterialChanged(MaterialBase material)
		{
			base.MaterialChanged(material);
			//TODO: The material doesn't have any effect?
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
					FlaxEngine.Object.Destroy(ref _task);
					FlaxEngine.Object.Destroy(ref _defaultOutput);
				}
				disposedValue = true;
			}
			base.Dispose(disposing);
		}

		#endregion IDisposable Support
	}
}