using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CartoonShader.Source.RenderingPipeline.RenderingOutput;
using FlaxEngine;
using FlaxEngine.Rendering;

namespace CartoonShader.Source.RenderingPipeline.RenderingTask
{
	public class CameraRenderer : IRenderer
	{
		public CameraRenderer(Camera camera, Vector2 size, string name = "")
		{
			Name = name;

			Output = new RenderOutput(size, this);

			if (!Task) Task = RenderTask.Create<SceneRenderTask>();
			Task.Enabled = false;
			Task.Camera = camera;
			Task.Output = Output.RenderTarget;

			// TODO: Enable the task!

			Task.Begin += OnRenderTaskBegin;
			//Task.Buffers = RenderBuffers.New(); // Pre-initialize the buffers
		}

		public string Name { get; set; }

		public MaterialRenderInputs Inputs { get; } = new MaterialRenderInputs(null);

		public SceneRenderTask Task { get; }

		RenderTask IRenderer.Task => Task;

		public RenderOutput Output { get; }

		public RenderOutput DepthBufferOutput { get; set; }

		public RenderOutput MotionVectorsOutput { get; set; }

		public Camera Camera => Task.Camera;

		private void OnRenderTaskBegin(SceneRenderTask task, GPUContext context)
		{
			// No need to dispose of the RenderTargets, _task takes care of it
			DepthBufferOutput = new RenderOutput(Task.Buffers.DepthBuffer, this);
			MotionVectorsOutput = new RenderOutput(Task.Buffers.MotionVectors, this);
			Task.Begin -= OnRenderTaskBegin;
		}

		#region IDisposable Support

		private bool _disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				if (disposing)
				{
					if (Task)
					{
						Task.Begin -= OnRenderTaskBegin;
						Task.Enabled = false;
					}
					Task?.Dispose();
					FlaxEngine.Object.Destroy(Task);
					FlaxEngine.Object.Destroy(Output.RenderTarget);
					Inputs.Material = null;
				}
				_disposedValue = true;
			}
		}

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
		}

		#endregion IDisposable Support
	}
}