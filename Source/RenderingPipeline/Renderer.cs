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
		[Serialize]
		protected int _order;

		[Serialize]
		protected MaterialBase _material;

		[Serialize]
		protected Vector2 _size;

		[NoSerialize]
		protected readonly Dictionary<string, RendererOutput> _outputs = new Dictionary<string, RendererOutput>();

		[NoSerialize]
		protected readonly Dictionary<string, RendererInput> _inputs = new Dictionary<string, RendererInput>();

		[NoSerialize]
		protected MaterialInstance _materialInstance;

		[NoSerialize]
		protected bool _enabled = false;

		[NoSerialize]
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

		[NoSerialize]
		public virtual RendererOutput DefaultOutput { get; protected set; }

		[NoSerialize]
		public IReadOnlyDictionary<string, RendererOutput> Outputs { get => _outputs; } //TODO: Naming convention for the default output?

		[NoSerialize]
		public IReadOnlyDictionary<string, RendererInput> Inputs { get => _inputs; }

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

		//TODO: Set the size!
		[NoSerialize]
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

		//TODO: Enable every renderer upon startup!
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

		public Renderer()
		{
		}

		/// <summary>
		/// Adds or replaces a RendererOutput
		/// </summary>
		/// <param name="rendererOutput"></param>
		protected void AddOutput(RendererOutput rendererOutput)
		{
			if (rendererOutput == null) return;

			// If it already has this RendererOutput, just return
			if (_outputs.TryGetValue(rendererOutput.Name, out RendererOutput existingOutput))
			{
				if (existingOutput.RenderTarget == rendererOutput.RenderTarget)
				{
					return;
				}
			}

			_outputs.Add(rendererOutput.Name, rendererOutput);
		}

		protected void RemoveOutput(string name)
		{
			_outputs.Remove(name);
		}

		protected virtual void Enable(bool enabled)
		{
			if (enabled)
			{
				MaterialChanged(Material);
				SizeChanged(Size);
				OrderChanged(Order);
			}
		}

		protected virtual void SizeChanged(Vector2 size)
		{
			if (!Enabled) return;
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

			Dictionary<string, RendererInput> newInputs = new Dictionary<string, RendererInput>();
			AddInputsFromMaterial(newInputs, _materialInstance.Parameters);
			AddInputs(newInputs);
			UpdateInputs(newInputs);
			newInputs.Clear();

			UpdateMaterialInputs();
		}

		/// <summary>
		/// Updates the _materialInstance-RenderTarget-parameter values
		/// </summary>
		private void UpdateMaterialInputs()
		{
			ActionRunner.Instance.AfterFirstUpdate(() =>
			{
				foreach (var input in Inputs.Values)
				{
					UpdateMaterialInput(input);
				}
			});
		}

		private void UpdateMaterialInput(RendererInput input)
		{
			if (input.RendererOutput == null || !input.RendererOutput.RenderTarget) return;
			//TODO: This might end up getting called before everything is ready?
			var materialParam = _materialInstance.GetParam(input.Name);
			if (materialParam != null)
			{
				materialParam.Value = input.RendererOutput.RenderTarget;
			}
		}

		private void AddInputsFromMaterial(Dictionary<string, RendererInput> newInputs, MaterialParameter[] materialParameters)
		{
			var inputParameters = materialParameters
				.Where(param => param.Type == MaterialParameterType.RenderTarget);

			foreach (var parameter in inputParameters)
			{
				// Add all the new material pararmeters
				newInputs.Add(parameter.Name, new RendererInput(parameter.Name, UpdateMaterialInput));
			}
		}

		protected virtual void AddInputs(Dictionary<string, RendererInput> newInputs)
		{
		}

		private void UpdateInputs(Dictionary<string, RendererInput> newInputs)
		{
			// Update the existing inputs and remove the outdated ones
			foreach (var inputName in _inputs.Keys.ToList())
			{
				if (newInputs.TryGetValue(inputName, out var newInput))
				{
					_inputs[inputName] = newInput;
				}
				else
				{
					_inputs.Remove(inputName);
				}
			}

			// Add the totally new inputs
			foreach (var newInput in newInputs)
			{
				string newInputName = newInput.Key;
				if (!_inputs.ContainsKey(newInputName))
				{
					_inputs.Add(newInputName, newInput.Value);
				}
			}
		}

		protected virtual void OrderChanged(int order)
		{
			if (!Enabled) return;
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