using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine.Rendering;

namespace CartoonShader.Source.RenderingPipeline
{
	public interface IRendererOutputs : IReadOnlyDictionary<string, RenderTarget>
	{
		// TODO?
	}

	public class RendererOutputs : IRendererOutputs
	{
		private readonly Dictionary<string, RenderTarget> _outputs = new Dictionary<string, RenderTarget>();

		public RendererOutputs()
		{
		}

		public int Count => _outputs.Count;

		public IEnumerable<string> Keys => _outputs.Keys;

		public IEnumerable<RenderTarget> Values => _outputs.Values;

		public RenderTarget this[string name]
		{
			get
			{
				return _outputs[name];
			}
		}

		public bool ContainsKey(string key) => _outputs.ContainsKey(key);

		public IEnumerator<KeyValuePair<string, RenderTarget>> GetEnumerator()
		{
			return _outputs
				.Select((keyValuePair) => new KeyValuePair<string, RenderTarget>(keyValuePair.Key, keyValuePair.Value))
				.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() => _outputs.GetEnumerator();

		public bool TryGetValue(string key, out RenderTarget value)
		{
			bool returnValue = _outputs.TryGetValue(key, out var rendererOutput);
			value = rendererOutput;
			return returnValue;
		}

		// TODO: Make this public? Or some other solution?
		public void SetOutput(string name, RenderTarget renderTarget)
		{
			if (name == null) return;

			if (_outputs.TryGetValue(name, out RenderTarget existingOutput))
			{
				if (existingOutput != renderTarget)
				{
					_outputs[name] = renderTarget;
				}
			}
			else
			{
				_outputs.Add(name, renderTarget);
			}
		}

		public void RemoveOutput(string name)
		{
			RenderTarget rt = _outputs[name];
			FlaxEngine.Object.Destroy(ref rt);
			_outputs.Remove(name);
		}
	}
}