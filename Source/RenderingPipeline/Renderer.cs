using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;
using FlaxEngine.Rendering;

namespace CartoonShader.Source.RenderingPipeline
{
	//TODO: Make this class abstract?
	public class Renderer : IDisposable
	{
		//TODO: [Serialize] and [NoSerialize]
		protected int _order;

		protected readonly Dictionary<string, RendererOutput> _outputs = new Dictionary<string, RendererOutput>();
		protected readonly Dictionary<string, RendererInput> _inputs = new Dictionary<string, RendererInput>();

		[Serialize]
		protected MaterialBase _material;

		[NoSerialize]
		protected MaterialInstance _materialInstance;

		protected Vector2 _size;
		protected bool _enabled;

		internal int Order
		{
			get => _order;
			set
			{
				if (_order != value)
				{
					_order = value;
					OrderChanged(_order);
				}
			}
		}

		public virtual RendererOutput DefaultOutput { get; protected set; }

		[NoSerialize]
		public IReadOnlyDictionary<string, RendererOutput> Outputs { get => _outputs; } //TODO: Naming convention for the default output?

		[NoSerialize]
		public IReadOnlyDictionary<string, RendererInput> Inputs { get => _inputs; } //TODO: Event when the inputs have changed!!

		/// <summary>
		/// The Material that will be used by this <see cref="Renderer"/> to render something
		/// </summary>
		[NoSerialize]
		public MaterialBase Material
		{
			get => _material;
			set
			{
				if (_material != value)
				{
					_material = value;
					MaterialChanged(_material);
				}
			}
		}

		public Vector2 Size
		{
			get => _size;
			set
			{
				if (_size != value)
				{
					_size = value;
					SizeChanged(_size);
				}
			}
		}

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

		protected virtual void Enable(bool enabled)
		{
			if (enabled)
			{
				MaterialChanged(Material);
				//SizeChanged(Size);
				//OrderChanged(Order);
			}
			throw new NotImplementedException();
		}

		protected virtual void SizeChanged(Vector2 size)
		{
			if (!Enabled) return;
			throw new NotImplementedException();
		}

		protected virtual void MaterialChanged(MaterialBase material)
		{
			if (!Enabled) return;

			material.WaitForLoaded();
			if (_materialInstance)
			{
				FlaxEngine.Object.Destroy(ref _materialInstance);
			}
			_materialInstance = material.CreateVirtualInstance();

			SetInputs(_materialInstance.Parameters);
		}

		private void SetInputs(MaterialParameter[] materialParameters)
		{
			//TODO: Add MaterialParameterType.RenderTargetArray and whatnot
			var inputParameters = materialParameters
				.Where(param => param.Type == MaterialParameterType.RenderTarget);

			HashSet<string> parameterNames = new HashSet<string>(
				inputParameters.Select(param => param.Name)
			);

			foreach (var inputName in _inputs.Keys.ToList())
			{
				// If the new material parameters don't have that one, get rid of it
				if (!parameterNames.Contains(inputName))
				{
					_inputs.Remove(inputName);
				}
			}

			foreach (var parameter in inputParameters)
			{
				// Add all the new material pararmeters
				if (!_inputs.ContainsKey(parameter.Name))
				{
					_inputs.Add(parameter.Name, new RendererInput(parameter.Name));
				}
			}
		}

		protected virtual void OrderChanged(int order)
		{
			if (!Enabled) return;
			throw new NotImplementedException();
		}

		#region IDisposable Support

		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects).
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

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