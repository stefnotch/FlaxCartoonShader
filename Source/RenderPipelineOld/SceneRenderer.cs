using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FlaxEngine;
using FlaxEngine.Rendering;

namespace CartoonShader.Source.RenderPipelineOld
{
	public class SceneRenderer : Renderer
	{
		/// <summary>
		/// Camera source
		/// </summary>
		public Camera SourceCamera;

		public override RenderTarget RenderOutput => _output;

		public RenderBuffers Buffers => _task.Buffers;

		private RenderTarget _output;
		private SceneRenderTask _task;

		public override void Initialize(Vector2 size, int order = -100000)
		{
			// Create backbuffer
			if (_output == null)
				_output = RenderTarget.New();
			_output.Init(PixelFormat.R8G8B8A8_UNorm, size);

			// Create rendering task
			if (_task == null)
				_task = RenderTask.Create<SceneRenderTask>();
			_task.Order = order;
			_task.Camera = SourceCamera;
			_task.Output = _output;
			_task.Enabled = false;

			ScriptUtils.Instance.AddSingleUpdate(() =>
			{
				_task.Enabled = true;
			});
		}

		private void OnDisable()
		{
			// Cleanup
			Destroy(ref _task);
			Destroy(ref _output);
		}

		protected override void SizeChanged(Vector2 newSize)
		{
			if (_output)
			{
				_output.Init(PixelFormat.R8G8B8A8_UNorm, newSize);
			}
		}
	}
}