using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;

namespace CartoonShader.Source.RenderingPipeline
{
	// Pretty hack-ish script
	public class ActionRunner : Script
	{
		private static ActionRunner _instance;
		private bool _isAfterFirstUpdate = false;
		private readonly TaskCompletionSource<bool> _promiseFirstUpdate = new TaskCompletionSource<bool>();
		private bool _cleaningUp = false;

		private ActionRunner()
		{
			if (_instance != null)
			{
				Debug.LogError("Instance is already set");
				_instance.Cleanup();
			}
			OnNextUpdate(() =>
			{
				_isAfterFirstUpdate = true;
				_promiseFirstUpdate.SetResult(true);
			});
		}

		private void OnDestroy()
		{
			Cleanup();
		}

		private void Cleanup()
		{
			if (_cleaningUp) return;
			Debug.Log("cleaner");
			if (!_isAfterFirstUpdate)
			{
				_promiseFirstUpdate.SetCanceled();
			}
			_cleaningUp = true;
			_instance = null;
			if (this.Actor)
			{
				Destroy(this.Actor);
			}
		}

		private void CreateContainerActor()
		{
			var containerActor = New<EmptyActor>();
			containerActor.HideFlags = HideFlags.FullyHidden;
			SceneManager.SpawnActor(containerActor);
			containerActor.AddScript(_instance);
		}

		public static ActionRunner Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = New<ActionRunner>();
					_instance.CreateContainerActor();
				}
				return _instance;
			}
		}

		private readonly LinkedList<Action> _actions = new LinkedList<Action>();

		public void OnNextUpdate(Action onSingleUpdate)
		{
			if (onSingleUpdate == null) return;

			bool isEmpty = _actions.Count <= 0;
			_actions.AddLast(onSingleUpdate);
			if (isEmpty)
			{
				Scripting.Update += ExecuteActions;
			}
		}

		//TODO: Fancy async/await tricks!

		public void AfterFirstUpdate(Action action)
		{
			if (_isAfterFirstUpdate) action?.Invoke();
			else OnNextUpdate(action);
		}

		public async Task FirstUpdate()
		{
			if (_isAfterFirstUpdate) return;

			await _promiseFirstUpdate.Task;
		}

		private void ExecuteActions()
		{
			List<Action> currentActions = _actions.ToList();
			_actions.Clear();

			foreach (var action in currentActions)
			{
				try
				{
					// TODO: Don't let 1 exception affect the rest
					action.Invoke();
				}
				catch (Exception ex)
				{
					Debug.LogError(ex);
				}
			}

			// TODO: Multithreaded code = race condition
			Scripting.Update -= ExecuteActions;
		}
	}
}