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
		private readonly LinkedList<Action> _actions = new LinkedList<Action>();
		private bool _isAfterFirstUpdate;
		private bool _isExiting;

		private readonly TaskCompletionSource<bool> _firstUpdatePromise = new TaskCompletionSource<bool>();

		protected ActionRunner()
		{
			if (_instance != null)
			{
				Debug.LogError("Instance is already set!");
				_instance.Scripting_Exit();
			}

			_isAfterFirstUpdate = false;
			FlaxEditor.Scripting.ScriptsBuilder.ScriptsReloadBegin += Scripting_Exit;
			Scripting.Exit += Scripting_Exit;

			OnNextUpdate(() =>
			{
				_isAfterFirstUpdate = true;
				_firstUpdatePromise.SetResult(true);
			});
		}

		private void Scripting_Update()
		{
			throw new NotImplementedException();
		}

		private void Scripting_Exit()
		{
			if (_isExiting) return;
			else _isExiting = true;

			if (_isAfterFirstUpdate == false)
			{
				_firstUpdatePromise.SetCanceled();
			}
			_isAfterFirstUpdate = false;
			_actions.Clear();

			_instance = null;
		}

		public static ActionRunner Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new ActionRunner();
				}
				return _instance;
			}
		}

		public void AfterFirstUpdate(Action action)
		{
			if (action == null) return;

			if (_isAfterFirstUpdate)
			{
				action.Invoke();
			}
			else
			{
				OnNextUpdate(action);
			}
		}

		public void OnNextUpdate(Action action)
		{
			if (action == null) return;

			bool isEmpty = _actions.Count <= 0;
			_actions.AddLast(action);
			if (isEmpty)
			{
				Scripting.Update += ExecuteActionsOnUpdate;
			}
		}

		public async Task FirstUpdate()
		{
			if (_isAfterFirstUpdate) return;

			await _firstUpdatePromise.Task;
		}

		private void ExecuteActionsOnUpdate()
		{
			List<Action> currentActions = _actions.ToList();
			_actions.Clear();

			foreach (var action in currentActions)
			{
				try
				{
					action.Invoke();
				}
				catch (Exception ex)
				{
					Debug.LogError(ex);
				}
			}

			// TODO: Multithreaded code = race condition?
			Scripting.Update -= ExecuteActionsOnUpdate;
		}
	}
}