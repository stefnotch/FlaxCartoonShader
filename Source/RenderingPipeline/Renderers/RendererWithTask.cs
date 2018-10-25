﻿using System;
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
		[NoSerialize]
		protected SceneRenderTask _task;

		[Serialize]
		protected int _order;

		[Serialize]
		protected Vector2 _size;

		[NoSerialize]
		protected bool _enabled = false;

		[NoSerialize]
		protected RendererOutput _defaultOutput;

		[NoSerialize]
		public virtual IRendererOutput DefaultOutput => _defaultOutput;

		public IReadOnlyDictionary<string, IRendererOutput> Outputs { get; private set; } = new Dictionary<string, IRendererOutput>();

		// TODO: Serialize & Deserialize this!!!
		[NoSerialize]
		public RendererInputs Inputs { get; } = new RendererInputs();

		[NoSerialize]
		internal int Order
		{
			get => _order;
			set
			{
				if (_order != value)
				{
					_order = value;
					if (Enabled) OrderChangedInternal(_order);
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
					Enable(_enabled);
				}
			}
		}

		private void SizeChangedInternal(Vector2 size)
		{
			if (Enabled) SizeChanged(size);
		}

		protected virtual void SizeChanged(Vector2 size)
		{
			if (DefaultOutput != null && DefaultOutput.RenderTarget)
			{
				DefaultOutput.RenderTarget.Init(PixelFormat.R8G8B8A8_UNorm, size);
			}
		}

		protected virtual void Enable(bool enabled)
		{
			if (enabled)
			{
				_defaultOutput.RenderTarget = RenderTarget.New();

				if (!_task) _task = RenderTask.Create<SceneRenderTask>();
				_task.Enabled = false;
				_task.Output = DefaultOutput.RenderTarget;

				SizeChangedInternal(Size);
				OrderChangedInternal(Order);
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

		/*
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
		*/

		#region IDisposable Support

		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					FlaxEngine.Object.Destroy(ref _task);
					RenderTarget renderTarget = _defaultOutput?.RenderTarget;
					FlaxEngine.Object.Destroy(ref renderTarget);
				}
				disposedValue = true;
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