﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;

namespace CartoonShader.Source.RenderingPipeline
{
	public class RenderToMaterial : IRendererDisplayer
	{
		[NoSerialize]
		protected RendererInputs _inputs = new RendererInputs(new string[] { "Default" });

		[Serialize]
		[Limit(1)]
		protected Vector2 _size;

		[Serialize]
		protected bool _enabled;

		public MaterialBase Material;

		public RenderToMaterial()
		{
			_inputs.RendererInputChanged += _inputs_RendererInputChanged;
		}

		private void _inputs_RendererInputChanged(string name, IRendererOutput rendererOutput)
		{
			_inputs_RendererInputChangedAsync(name, rendererOutput);
		}

		private async void _inputs_RendererInputChangedAsync(string name, IRendererOutput rendererOutput)
		{
			// Not sure which one is "better"
			await ActionRunner.Instance.FirstUpdate();
			//ActionRunner.Instance.AfterFirstUpdate(() =>
			//{
			Material.WaitForLoaded();
			Debug.Log("RendererInputChanged - Material Set");
			var param = Material?.GetParam(name);
			if (param != null)
			{
				Debug.Log("RendererInputChanged - Material Set!!");
				param.Value = rendererOutput?.RenderTarget;
			}
			//});
		}

		[NoSerialize]
		public RendererInputs Inputs => _inputs;

		[NoSerialize]
		public Vector2 Size
		{
			get => _size;
			set
			{
				if (_size != value)
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

		private void SizeChangedInternal(Vector2 size)
		{
			if (Enabled) SizeChanged(size);
		}

		protected void SizeChanged(Vector2 size)
		{
		}

		protected void EnableChanged(bool enabled)
		{
			if (enabled)
			{
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
					// dispose managed state (managed objects).
					_inputs.RendererInputChanged -= _inputs_RendererInputChanged;
				}

				// free unmanaged resources (unmanaged objects) and override a finalizer below.
				// set large fields to null.

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