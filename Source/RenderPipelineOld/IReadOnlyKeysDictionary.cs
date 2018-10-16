using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CartoonShader.Source.RenderPipelineOld
{
	public interface IReadOnlyKeysDictionary<TKey, TValue> : IReadOnlyCollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>
	{
		TValue this[TKey key] { get; set; }

		IEnumerable<TKey> Keys { get; }
		IEnumerable<TValue> Values { get; }

		bool ContainsKey(TKey key);

		bool TryGetValue(TKey key, out TValue value);
	}
}