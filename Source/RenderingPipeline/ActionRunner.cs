using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;

namespace CartoonShader.Source.RenderingPipeline
{
	public class ActionRunner
	{
		private static ActionRunner _instance;
		private bool _isAfterFirstUpdate = false;
		private readonly TaskCompletionSource<bool> _promiseFirstUpdate = new TaskCompletionSource<bool>();

		private ActionRunner()
		{
			OnNextUpdate(() =>
			{
				_isAfterFirstUpdate = true;
				_promiseFirstUpdate.SetResult(true);
			});
		}

		public static ActionRunner Instance
		{
			get
			{
				if (_instance == null) _instance = new ActionRunner();
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