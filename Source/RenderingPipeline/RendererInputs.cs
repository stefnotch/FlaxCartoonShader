using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine.Rendering;

namespace CartoonShader.Source.RenderingPipeline
{
	//TODO: IObservable
	//TODO: I don't really need this interface, do I?

	public interface IRendererInputs : IReadOnlyDictionary<string, RenderTarget>, IReadOnlyCollection<KeyValuePair<string, RenderTarget>>, IEnumerable<KeyValuePair<string, RenderTarget>>
	{
		new RenderTarget this[string key] { get; set; }
	}

	public class RendererInputs : IRendererInputs
	{
		private readonly Dictionary<string, RenderTarget> _rendererInputs = new Dictionary<string, RenderTarget>();

		/// <summary>
		/// Raised whenever a RenderTarget is changed
		/// </summary>
		public event Action<string, RenderTarget> RendererInputChanged;

		public RendererInputs()
		{
		}

		public RendererInputs(IEnumerable<string> rendererInputNames)
		{
			UpdateInputs(rendererInputNames);
		}

		public RenderTarget this[string key]
		{
			get
			{
				return _rendererInputs[key];
			}
			set
			{
				// Set the value
				_rendererInputs[key] = value;

				// Raise the event
				RendererInputChanged?.Invoke(key, value);
			}
		}

		// TODO: Only the owner object should be able to call this
		// TODO: Create an interface and stuffz
		public void UpdateInputs(IEnumerable<string> rendererInputNames, IEnumerable<string> previousRendererInputNames = null)
		{
			// TODO: Previous input caching and whatnot
			//HashSet<string> newRendererInputNames = new HashSet<string>(rendererInputNames);
			if (previousRendererInputNames != null)
			{
				foreach (var name in previousRendererInputNames)
				{
					_rendererInputs.Remove(name);
				}
			}

			foreach (var name in rendererInputNames)
			{
				_rendererInputs.Add(name, null);
			}
		}

		public IEnumerable<string> Keys => _rendererInputs.Keys;

		public IEnumerable<RenderTarget> Values => _rendererInputs.Values;

		public int Count => _rendererInputs.Count;

		public bool ContainsKey(string key) => _rendererInputs.ContainsKey(key);

		public IEnumerator<KeyValuePair<string, RenderTarget>> GetEnumerator() => _rendererInputs.GetEnumerator();

		public bool TryGetValue(string key, out RenderTarget value) => _rendererInputs.TryGetValue(key, out value);

		IEnumerator IEnumerable.GetEnumerator() => _rendererInputs.GetEnumerator();
	}
}