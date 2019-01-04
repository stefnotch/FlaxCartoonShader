using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;
using FlaxEngine.Rendering;

namespace CartoonShader.Source.RenderingPipeline.RenderDisplayer
{
	public class RenderToMaterial : IRendererDisplayer
	{
		[NoSerialize]
		protected RendererInputs _inputs = new RendererInputs(new string[] { "Default" });

		private readonly Dictionary<string, IDisposable> _disposables = new Dictionary<string, IDisposable>();

		[Serialize]
		protected bool _enabled;

		[Serialize]
		private MaterialBase _material;

		public RenderToMaterial()
		{
			_inputs.RendererInputChanged += _inputs_RendererInputChanged;
		}

		private void _inputs_RendererInputChanged(string name, RenderOutput rendererOutput)
		{
			_inputs_RendererInputChangedAsync(name, rendererOutput);
		}

		private void _inputs_RendererInputChangedAsync(string name, RenderOutput rendererOutput)
		{
			if (_disposables.TryGetValue(name, out IDisposable value))
			{
				value.Dispose();
			}
			_disposables.Add(name,
			rendererOutput.Subscribe(
				(renderOutput) => SetMaterialInput(name, renderOutput?.RenderTarget),
				() => SetMaterialInput(name, null)
			));
		}

		private void SetMaterialInput(string name, RenderTarget renderTarget)
		{
			if (!Material) return;

			Material.WaitForLoaded();
			var param = Material?.GetParam(name);
			if (param != null)
			{
				param.Value = renderTarget;
			}
			//});
		}

		[NoSerialize]
		public IRendererInputs Inputs => _inputs;

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

		[NoSerialize]
		public MaterialBase Material
		{
			get => _material;
			set
			{
				if (_material != value)
				{
					_material = value;
					MaterialChangedInternal(_material);
				}
			}
		}

		private void MaterialChangedInternal(MaterialBase material)
		{
			if (Enabled) MaterialChanged(material);
		}

		private void MaterialChanged(MaterialBase material)
		{
			foreach (var input in _inputs)
			{
				_inputs_RendererInputChanged(input.Key, input.Value);
			}
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