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

		// TODO: Dispose of all the outputs

		public RendererWithTask()
		{
			if (!_task) _task = RenderTask.Create<SceneRenderTask>();
			_task.Enabled = false;

			_defaultOutput = new RenderOutput(RenderTarget.New(), this);

			_task.Output = _defaultOutput.RenderTarget;

			_inputs.RendererInputChanged += RendererInputChanged;
		}

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

		//TODO: Set the size! (from some other class)
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

		//TODO: Enable every renderer upon startup! (from some other class)
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

		// TODO: Improve this (as soon as you turn on an output, the pipeline gets Enabled automatically!!!)
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

		/*protected void SetOutput(string name, RenderOutput RenderOutput)
		{
			if (name == null) return;

			if (_outputs.TryGetValue(name, out IRendererOutput existingOutput))
			{
				if (existingOutput.RenderOutput != RenderOutput)
				{
					RenderOutput rt = existingOutput.RenderOutput;
					FlaxEngine.Object.Destroy(ref rt);

					_outputs[name].RenderOutput = RenderOutput;
				}
			}
			else
			{
				var output = new RendererOutput(name);
				output.RenderOutput = RenderOutput;
				_outputs.Add(name, output);
			}
		}

		protected void RemoveOutput(string name)
		{
			RenderOutput rt = _outputs[name].RenderOutput;
			FlaxEngine.Object.Destroy(ref rt);
			_outputs[name].RenderOutput = null;
			_outputs.Remove(name);
		}*/

		/*
		protected RenderOutput _defaultOutput;

		public override sealed RendererOutput DefaultOutput { get; protected set; }

		protected override void Enable(bool enabled)
		{
			if (enabled)
			{
				if (!_defaultOutput)
				{
					_defaultOutput = RenderOutput.New();
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
		*/

		#region IDisposable Support

		private bool _disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				if (disposing)
				{
					// TODO: Send a bunch of null render targets?
					_inputs.RendererInputChanged -= RendererInputChanged;
					Enabled = false;
					_task?.Dispose();
					FlaxEngine.Object.Destroy(ref _task);
					FlaxEngine.Object.Destroy(_defaultOutput.RenderTarget);
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