using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;
using FlaxEngine.Rendering;

namespace CartoonShader.Source.RenderingPipeline.Renderers
{
	// TODO: Merge this with Renderer.cs?
	public class RendererWithTask : Renderer
	{
		protected SceneRenderTask _task;
		protected RenderTarget _defaultOutput;

		public override sealed RendererOutput DefaultOutput { get; protected set; }

		protected override void Enable(bool enabled)
		{
			if (enabled)
			{
				if (!_defaultOutput)
				{
					_defaultOutput = RenderTarget.New();
					AddOutput(new RendererOutput("Default", _defaultOutput));
				}
				if (!_task) _task = RenderTask.Create<SceneRenderTask>();
				_task.Enabled = false;
			}
			else
			{
				EnableRenderTask(false);
			}
			base.Enable(enabled);
			if (enabled)
			{
				_task.Output = _defaultOutput;

				ActionRunner.Instance.OnUpdate_Once(() =>
				{
					EnableRenderTask(true);
				});
			}
			else
			{
			}
		}

		protected virtual void EnableRenderTask(bool enabled)
		{
			if (enabled)
			{
				_task.Enabled = true;
			}
			else
			{
				_task.Enabled = false;
			}
		}

		protected override void MaterialChanged(MaterialBase material)
		{
			base.MaterialChanged(material);
			if (!material) return;
		}

		protected override void OrderChanged(int order)
		{
			base.OrderChanged(order);

			if (Enabled && _task)
			{
				_task.Order = order;
			}
		}

		protected override void SizeChanged(Vector2 size)
		{
			base.SizeChanged(size);

			if (Enabled && _defaultOutput && size.X >= 1 && size.Y >= 1)
			{
				_defaultOutput.Init(PixelFormat.R8G8B8A8_UNorm, size);
			}
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