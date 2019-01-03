using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;
using FlaxEngine.Rendering;

namespace CartoonShader.Source.RenderingPipeline.Renderers
{
	public class SceneRenderer : RendererWithTask
	{
		/// <summary>
		/// Camera source
		/// </summary>
		public Camera SourceCamera;

		public RenderTarget DepthBufferOutput { get; protected set; }
		public RenderTarget MotionVectorsOutput { get; protected set; }

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
					// No need to dispose of those things, _task takes care of it
					DepthBufferOutput = _task.Buffers.DepthBuffer;
					MotionVectorsOutput = _task.Buffers.MotionVectors;
				});
			}
		}
	}
}