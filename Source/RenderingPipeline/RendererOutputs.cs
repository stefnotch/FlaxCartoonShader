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
		event Action<string, IRendererOutput> OutputChanged;

		// TODO
	}

	public class RendererOutputs : IRendererOutputs
	{
		public IRendererOutput Default;
		private readonly Dictionary<string, IRendererOutput> _outputs = new Dictionary<string, IRendererOutput>();

		public RendererOutputs()
		{
		}

		public event Action<string, IRendererOutput> OutputChanged;

		public int Count => throw new NotImplementedException();

		public IEnumerable<string> Keys => throw new NotImplementedException();

		public IEnumerable<IRendererOutput> Values => throw new NotImplementedException();

		// something like this
		public IRendererOutput this[string name]
		{
			get
			{
				return null;
			}
			/*set
			{
				if (value == null)
				{
					RemoveOutput(name);
				}
				else
				{
				}
			}*/
		}

		/*
		public RenderTarget this[string name]
		{
			get
			{
				return null;
			}
		}
		*/

		public bool ContainsKey(string key)
		{
			throw new NotImplementedException();
		}

		public IEnumerator<KeyValuePair<string, IRendererOutput>> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		public bool TryGetValue(string key, out IRendererOutput value)
		{
			throw new NotImplementedException();
		}

		// TODO: Should I really totally destroy it???
		private void RemoveOutput(string name)
		{
			RenderTarget rt = _outputs[name].RenderTarget;
			FlaxEngine.Object.Destroy(ref rt);
			_outputs[name].RenderTarget = null;
			_outputs.Remove(name);
		}

		// TODO: Make this public?
		private void SetOutput(string name, RenderTarget renderTarget)
		{
			if (name == null) return;

			if (_outputs.TryGetValue(name, out IRendererOutput existingOutput))
			{
				if (existingOutput.RenderTarget != renderTarget)
				{
					// TODO: Should I seriously destroy it?
					RenderTarget rt = existingOutput.RenderTarget;
					FlaxEngine.Object.Destroy(ref rt);

					_outputs[name].RenderTarget = renderTarget;
				}
			}
			else
			{
				var output = new RendererOutput(name)
				{
					RenderTarget = renderTarget
				};
				_outputs.Add(name, output);
			}
		}
	}
}