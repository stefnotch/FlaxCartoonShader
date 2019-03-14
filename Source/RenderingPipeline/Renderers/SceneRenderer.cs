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

		public RenderOutput DepthBufferOutput { get; }
		public RenderOutput MotionVectorsOutput { get; }

		public SceneRenderer()
		{
			DepthBufferOutput = new RenderOutput(this);
			MotionVectorsOutput = new RenderOutput(this);

			_simpleDisposer.DisposeLater(DepthBufferOutput);
			_simpleDisposer.DisposeLater(MotionVectorsOutput);
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
				_task.Begin -= OnRenderTaskBegin;
				_task.Begin += OnRenderTaskBegin;
			}
			else
			{
				_task.Begin -= OnRenderTaskBegin;
			}
		}

		private void OnRenderTaskBegin(SceneRenderTask task, GPUContext context)
		{
			// No need to dispose of the RenderTargets, _task takes care of it
			DepthBufferOutput.RenderTarget = _task.Buffers.DepthBuffer;
			MotionVectorsOutput.RenderTarget = _task.Buffers.MotionVectors;
		}
	}
}