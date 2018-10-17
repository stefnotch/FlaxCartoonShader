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
					DefaultOutput = new RendererOutput("Default", _defaultOutput);
					_outputs["Default"] = DefaultOutput;
				}
				if (!_task) _task = RenderTask.Create<SceneRenderTask>();
				_task.Enabled = false;
			}
			else
			{
				_task.Enabled = false;
			}
			base.Enable(enabled);
			if (enabled)
			{
				_task.Output = _defaultOutput;
			}
			else
			{
			}
		}

		protected override void MaterialChanged(MaterialBase material)
		{
			base.MaterialChanged(material);
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

			if (Enabled && _defaultOutput)
			{
				_defaultOutput.Init(PixelFormat.R8G8B8A8_UNorm, size);
			}
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			FlaxEngine.Object.Destroy(ref _task);
			FlaxEngine.Object.Destroy(ref _defaultOutput);
		}
	}
}