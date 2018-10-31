using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartoonShader.Source
{
	public class Disposer
	{
		private LinkedList<IDisposable> _toDisposeOnDestroy = new LinkedList<IDisposable>();

		public void Dispose(FlaxEngine.Object toDispose)
		{
			Dispose(toDispose);
		}

		public void Dispose(ref FlaxEngine.Object toDispose)
		{
			FlaxEngine.Object.Destroy(ref toDispose);
		}

		public void Dispose(IDisposable toDispose)
		{
			Dispose(ref toDispose);
		}

		public void Dispose(ref IDisposable toDispose)
		{
			toDispose?.Dispose();
			toDispose = null;
		}

		public void DisposeOnDestroy()
		{
		}

		public void DisposeOnDisable()
		{
		}
	}
}