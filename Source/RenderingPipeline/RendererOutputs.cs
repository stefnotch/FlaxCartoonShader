using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine.Rendering;

namespace CartoonShader.Source.RenderingPipeline
{
	public interface IRendererOutputs : IReadOnlyDictionary<string, IRendererOutput>
	{
		// TODO?
	}

	public class RendererOutputs : IRendererOutputs
	{
		private readonly Dictionary<string, RendererOutput> _outputs = new Dictionary<string, RendererOutput>();

		public RendererOutputs()
		{
		}

		public int Count => _outputs.Count;

		public IEnumerable<string> Keys => _outputs.Keys;

		public IEnumerable<IRendererOutput> Values => _outputs.Values;

		public IRendererOutput this[string name]
		{
			get
			{
				return _outputs[name];
			}
		}

		public bool ContainsKey(string key) => _outputs.ContainsKey(key);

		public IEnumerator<KeyValuePair<string, IRendererOutput>> GetEnumerator()
		{
			return _outputs
				.Select((keyValuePair) => new KeyValuePair<string, IRendererOutput>(keyValuePair.Key, keyValuePair.Value))
				.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() => _outputs.GetEnumerator();

		public bool TryGetValue(string key, out IRendererOutput value)
		{
			bool returnValue = _outputs.TryGetValue(key, out var rendererOutput);
			value = rendererOutput;
			return returnValue;
		}

		// TODO: Make this public? Or some other solution?
		public void SetOutput(string name, RenderTarget renderTarget)
		{
			if (name == null) return;

			if (_outputs.TryGetValue(name, out RendererOutput existingOutput))
			{
				if (existingOutput.RenderTarget != renderTarget)
				{
					_outputs[name].RenderTarget = renderTarget;
				}
			}
			else
			{
				var output = new RendererOutput()
				{
					RenderTarget = renderTarget
				};
				_outputs.Add(name, output);
			}
		}

		public void RemoveOutput(string name)
		{
			RenderTarget rt = _outputs[name].RenderTarget;
			FlaxEngine.Object.Destroy(ref rt);
			_outputs[name].RenderTarget = null;
			_outputs.Remove(name);
		}
	}
}