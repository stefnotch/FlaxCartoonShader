using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;
using FlaxEngine.Rendering;

namespace CartoonShader.Source.RenderingPipeline.Renderers
{
	public class RendererWithTask : IRenderer
	{
		[Serialize]
		protected int _order;

		[Serialize]
		protected Vector2 _size = Vector2.One;

		[NoSerialize]
		protected bool _enabled = false;

		[NoSerialize]
		protected SceneRenderTask _task;

		[NoSerialize]
		protected RenderOutput _defaultOutput;

		[NoSerialize]
		protected readonly RendererInputs _inputs = new RendererInputs();

		protected SimpleDisposer _simpleDisposer = new SimpleDisposer();

		public RendererWithTask()
		{
			if (!_task) _task = RenderTask.Create<SceneRenderTask>();
			_task.Enabled = false;

			_defaultOutput = new RenderOutput(this);
			_defaultOutput.RenderTarget = RenderTarget.New();

			_task.Output = _defaultOutput.RenderTarget;

			_inputs.RendererInputChanged += RendererInputChanged;
		}

		[NoSerialize]
		public SceneRenderTask SceneRenderTask => _task;

		[NoSerialize]
		public RenderOutput DefaultOutput => _defaultOutput;

		// TODO: Serialize & Deserialize this!!!
		[NoSerialize]
		public IRendererInputs Inputs => _inputs;

		[NoSerialize]
		internal int Order
		{
			get => _order;
			set
			{
				if (_order != value)
				{
					_order = value;
					OrderChangedInternal(_order);
				}
			}
		}

		[NoSerialize]
		public Vector2 Size
		{
			get => _size;
			set
			{
				if (_size != value && value.X >= 1 && value.Y >= 1)
				{
					_size = value;
					SizeChangedInternal(_size);
				}
			}
		}

		[NoSerialize]
		public bool Enabled
		{
			get => _enabled;
			set
			{
				if (_enabled != value)
				{
					_enabled = value;
					EnableChanged(_enabled);
				}
			}
		}

		public string Name { get; set; }

		protected virtual void RendererInputChanged(string name, RenderOutput newRendererOutput)
		{
		}

		private void SizeChangedInternal(Vector2 size)
		{
			if (Enabled) SizeChanged(size);
		}

		protected virtual void SizeChanged(Vector2 size)
		{
			if (DefaultOutput.RenderTarget)
			{
				DefaultOutput.RenderTarget.Init(PixelFormat.R8G8B8A8_UNorm, size);
			}
		}

		protected virtual void EnableChanged(bool enabled)
		{
			if (enabled)
			{
				SizeChangedInternal(Size);
				OrderChangedInternal(Order);

				ActionRunner.Instance.OnNextUpdate(() =>
				{
					EnableRenderTask(true);
				});
			}
			else
			{
				EnableRenderTask(false);
			}
		}

		protected virtual void EnableRenderTask(bool enabled)
		{
			if (enabled)
			{
				if (_task) _task.Enabled = true;
			}
			else
			{
				if (_task) _task.Enabled = false;
			}
		}

		private void OrderChangedInternal(int order)
		{
			if (Enabled) OrderChanged(order);
		}

		protected virtual void OrderChanged(int order)
		{
			if (_task)
			{
				_task.Order = order;
			}
		}

		#region IDisposable Support

		private bool _disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				if (disposing)
				{
					_inputs.RendererInputChanged -= RendererInputChanged;
					Enabled = false;
					_task?.Dispose();
					FlaxEngine.Object.Destroy(ref _task);
					FlaxEngine.Object.Destroy(_defaultOutput.RenderTarget);
					_defaultOutput.Dispose();
					_defaultOutput = null;

					_simpleDisposer.Dispose();
					_simpleDisposer = null;
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