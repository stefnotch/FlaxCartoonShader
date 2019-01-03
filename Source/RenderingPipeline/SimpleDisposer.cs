using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartoonShader.Source.RenderingPipeline
{
	/// <summary>
	/// Disposes of all the objects it holds.
	///
	/// However, it does not set anything to null, unlike <see cref="FlaxEngine.Object.Destroy{T}(ref T, float)"/>
	/// </summary>
	public class SimpleDisposer : IDisposable
	{
		// TODO: Weak references??

		private LinkedList<FlaxEngine.Object> _flaxObjects = new LinkedList<FlaxEngine.Object>();
		private LinkedList<IDisposable> _disposables = new LinkedList<IDisposable>();

		public void DisposeLater(FlaxEngine.Object flaxObject)
		{
			if (_disposedValue) throw new ObjectDisposedException(nameof(SimpleDisposer));

			_flaxObjects.AddLast(flaxObject);
		}

		public void DisposeLater(IDisposable disposeable)
		{
			if (_disposedValue) throw new ObjectDisposedException(nameof(SimpleDisposer));

			_disposables.AddLast(disposeable);
		}

		#region IDisposable Support

		private bool _disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects).
					foreach (var flaxObject in _flaxObjects)
					{
						FlaxEngine.Object.Destroy(flaxObject);
					}
					foreach (var disposeable in _disposables)
					{
						disposeable.Dispose();
					}
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.
				_flaxObjects = null;
				_disposables = null;

				_disposedValue = true;
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