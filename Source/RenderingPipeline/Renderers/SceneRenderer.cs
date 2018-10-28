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

		public SceneRenderer() : base()
		{
			_outputs.SetOutput(DepthBuffer, null);
			_outputs.SetOutput(MotionVectors, null);
		}

		protected override void EnableChanged(bool enabled)
		{
			base.EnableChanged(enabled);
			if (enabled)
			{
				_task.Camera = SourceCamera;
			}
		}

		protected override void EnableRenderTask(bool enabled)
		{
			base.EnableRenderTask(enabled);
			if (enabled)
			{
				ActionRunner.Instance.OnNextUpdate(() =>
				{
					_outputs.SetOutput(DepthBuffer, _task.Buffers.DepthBuffer);
					_outputs.SetOutput(MotionVectors, _task.Buffers.MotionVectors);
				});
			}
		}
	}
}