using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartoonShader.Source.RenderingPipeline
{
	//TODO: IObservable
	public class RendererInputs : IReadOnlyDictionary<string, IRendererOutput>, IReadOnlyCollection<KeyValuePair<string, IRendererOutput>>, IEnumerable<KeyValuePair<string, IRendererOutput>>
	{
		private readonly Dictionary<string, IRendererOutput> _rendererInputs = new Dictionary<string, IRendererOutput>();

		private readonly Dictionary<string, Action<IRendererOutput>> _rendererChangeListeners = new Dictionary<string, Action<IRendererOutput>>();

		public event Action<string, IRendererOutput> RendererInputChanged;

		public RendererInputs()
		{
		}

		public RendererInputs(IEnumerable<string> rendererInputNames)
		{
			SetInputs(rendererInputNames);
		}

		// TODO: Only the owner object should be able to call this
		// TODO: Create an interface and stuffz
		public void SetInputs(IEnumerable<string> rendererInputNames)
		{
			// TODO: Previous inputs...
			foreach (var key in _rendererInputs.Keys)
			{
				DetatchListener(key);
			}
			_rendererInputs.Clear();
			_rendererChangeListeners.Clear();
			foreach (var name in rendererInputNames)
			{
				_rendererInputs.Add(name, null);
				_rendererChangeListeners.Add(name, null);
			}
		}

		public IRendererOutput this[string key]
		{
			get
			{
				return _rendererInputs[key];
			}
			set
			{
				// Detatch previous listener
				DetatchListener(key);

				// Set the value
				_rendererInputs[key] = value;

				// Attach new listener
				AttatchListener(key);

				// Raise the event
				RendererInputChanged?.Invoke(key, value);
			}
		}

		private void AttatchListener(string key)
		{
			Action<IRendererOutput> rtChangedListener = (IRendererOutput rendererOutput) =>
			{
				RendererInputChanged?.Invoke(key, rendererOutput);
			};
			_rendererChangeListeners[key] = rtChangedListener;
			_rendererInputs[key].RenderTargetChanged += rtChangedListener;
		}

		private void DetatchListener(string key)
		{
			if (_rendererInputs[key] != null)
			{
				if (_rendererChangeListeners[key] != null)
				{
					_rendererInputs[key].RenderTargetChanged -= _rendererChangeListeners[key];
				}
			}
		}

		public IEnumerable<string> Keys => _rendererInputs.Keys;

		public IEnumerable<IRendererOutput> Values => _rendererInputs.Values;

		public int Count => _rendererInputs.Count;

		public bool ContainsKey(string key) => _rendererInputs.ContainsKey(key);

		public IEnumerator<KeyValuePair<string, IRendererOutput>> GetEnumerator() => _rendererInputs.GetEnumerator();

		public bool TryGetValue(string key, out IRendererOutput value) => _rendererInputs.TryGetValue(key, out value);

		IEnumerator IEnumerable.GetEnumerator() => _rendererInputs.GetEnumerator();
	}
}