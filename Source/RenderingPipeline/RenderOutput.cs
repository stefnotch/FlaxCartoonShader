using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine.Rendering;

namespace CartoonShader.Source.RenderingPipeline
{
	// Not much of a choice, the other option would have been even worse
	/// <summary>
	/// Warning: You have to dispose of the underlying <see cref="RenderTarget"/> yourself
	/// </summary>
	public class RenderOutput : IObservable<RenderOutput>, IDisposable
	{
		private RenderTarget _renderTarget;
		private readonly List<IObserver<RenderOutput>> _observers = new List<IObserver<RenderOutput>>();

		public RenderOutput(IRenderer renderer)
		{
			Renderer = renderer;
		}

		public readonly IRenderer Renderer;

		public RenderTarget RenderTarget
		{
			get
			{
				return _renderTarget;
			}
			set
			{
				_renderTarget = value;
				RenderTargetChanged(_renderTarget);
			}
		}

		public IDisposable Subscribe(IObserver<RenderOutput> observer)
		{
			if (!_observers.Contains(observer))
			{
				_observers.Add(observer);
				observer.OnNext(this);
			}
			return new Unsubscriber(_observers, observer);
		}

		private class Unsubscriber : IDisposable
		{
			private List<IObserver<RenderOutput>> _observers;
			private IObserver<RenderOutput> _observer;

			public Unsubscriber(List<IObserver<RenderOutput>> observers, IObserver<RenderOutput> observer)
			{
				_observers = observers;
				_observer = observer;
			}

			public void Dispose()
			{
				if (_observer != null && _observers.Contains(_observer))
				{
					_observers.Remove(_observer);
				}
			}
		}

		private void RenderTargetChanged(RenderTarget renderTarget)
		{
			foreach (var observer in _observers)
			{
				observer.OnNext(this);
			}
		}

		/// <summary>
		/// Calls <see cref="IObserver{T}.OnCompleted"/>
		/// </summary>
		public void Dispose()
		{
			foreach (var observer in _observers)
			{
				observer.OnCompleted();
			}
			_observers.Clear();
		}
	}

	public static class IObservableExtensions
	{
		public static IDisposable Subscribe<T>(this IObservable<T> observable, Action<T> onNext, Action onCompleted)
		{
			return observable.Subscribe(new Observer<T>(onNext, null, onCompleted));
		}

		private class Observer<T> : IObserver<T>
		{
			private readonly Action<T> _onNext;
			private readonly Action<Exception> _onError;
			private readonly Action _onCompleted;

			public Observer(Action<T> onNext, Action<Exception> onError = null, Action onCompleted = null)
			{
				_onNext = onNext;
				_onError = onError;
				_onCompleted = onCompleted;
			}

			public void OnCompleted()
			{
				_onCompleted?.Invoke();
			}

			public void OnError(Exception error)
			{
				_onError?.Invoke(error);
			}

			public void OnNext(T value)
			{
				_onNext?.Invoke(value);
			}
		}
	}
}