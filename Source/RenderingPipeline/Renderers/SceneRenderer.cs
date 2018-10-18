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

		protected override void Enable(bool enabled)
		{
			base.Enable(enabled);
			const string DepthBuffer = "DepthBuffer";
			const string MotionVectors = "MotionVectors";
			if (enabled)
			{
				_task.Camera = SourceCamera;

				AddOutput(new RendererOutput(DepthBuffer, _task.Buffers.DepthBuffer));
				AddOutput(new RendererOutput(MotionVectors, _task.Buffers.MotionVectors));
			}
		}

		protected override void MaterialChanged(MaterialBase material)
		{
			base.MaterialChanged(material);
		}

		protected override void OrderChanged(int order)
		{
			base.OrderChanged(order);
		}

		protected override void SizeChanged(Vector2 size)
		{
			base.SizeChanged(size);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}
	}
}