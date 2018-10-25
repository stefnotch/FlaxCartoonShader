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
				SetOutput(DepthBuffer, null);
				SetOutput(MotionVectors, null);
			}
		}

		protected override void EnableRenderTask(bool enabled)
		{
			base.EnableRenderTask(enabled);
			if (enabled)
			{
				SetOutput(DepthBuffer, _task.Buffers.DepthBuffer);
				SetOutput(MotionVectors, _task.Buffers.MotionVectors);
			}
		}
	}
}