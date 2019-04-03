using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CartoonShader.Source.RenderingPipeline.RenderingOutput;
using FlaxEngine;
using FlaxEngine.Rendering;

namespace CartoonShader.Source.RenderingPipeline.RenderingTask
{
	public class MaterialRenderInputs : IReadOnlyDictionary<string, RenderOutput>, IReadOnlyCollection<KeyValuePair<string, RenderOutput>>, IEnumerable<KeyValuePair<string, RenderOutput>>
	{
		private MaterialBase _material;
		private readonly Dictionary<string, RenderOutput> _rendererInputs = new Dictionary<string, RenderOutput>();

		public static readonly string DefaultInputName = "Image";

		public MaterialRenderInputs(MaterialBase material)
		{
			material?.WaitForLoaded();
			Material = material;
		}

		public RenderOutput this[string key]
		{
			get
			{
				return _rendererInputs[key];
			}
			set
			{
				// Set the value
				_rendererInputs[key] = value;
				// Update the material
				SetMaterialParam(_material, key, value);
			}
		}

		public MaterialBase Material
		{
			get { return _material; }
			set
			{
				_material = value;
				UpdateRenderInputs(_material);
			}
		}

		private void UpdateRenderInputs(MaterialBase newMaterial)
		{
			var oldNames = _rendererInputs.Keys.ToList();
			var newNames = GetRenderInputNames(newMaterial);

			foreach (string newName in newNames)
			{
				if (_rendererInputs.TryGetValue(newName, out RenderOutput renderOutput))
				{
					// Update the material
					SetMaterialParam(newMaterial, newName, renderOutput);
				}
				else
				{
					// Add the new key
					_rendererInputs.Add(newName, null);
				}
			}

			foreach (string oldName in oldNames)
			{
				if (!newNames.Contains(oldName))
				{
					// Remove the old key
					_rendererInputs.Remove(oldName);
				}
			}
		}

		private void SetMaterialParam(MaterialBase material, string key, RenderOutput renderOutput)
		{
			if (!material || renderOutput == null) return;

			// Update the material
			var param = _material?.GetParam(key);
			if (param != null)
			{
				param.Value = renderOutput.RenderTarget;
			}
		}

		private IEnumerable<string> GetRenderInputNames(MaterialBase material)
		{
			if (material == null) return Enumerable.Empty<string>();

			var renderInputNames = material.Parameters
				.Where(param => param.Type == MaterialParameterType.RenderTarget)
				.Select(param => param.Name);

			return renderInputNames;
		}

		public IEnumerable<string> Keys => _rendererInputs.Keys;

		public IEnumerable<RenderOutput> Values => _rendererInputs.Values;

		public int Count => _rendererInputs.Count;

		public bool ContainsKey(string key) => _rendererInputs.ContainsKey(key);

		public IEnumerator<KeyValuePair<string, RenderOutput>> GetEnumerator() => _rendererInputs.GetEnumerator();

		public bool TryGetValue(string key, out RenderOutput value) => _rendererInputs.TryGetValue(key, out value);

		IEnumerator IEnumerable.GetEnumerator() => _rendererInputs.GetEnumerator();
	}
}