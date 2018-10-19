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

		private ActionRunner()
		{
			OnUpdate_Once(() => _isAfterFirstUpdate = true);
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

		public void OnUpdate_Once(Action onSingleUpdate)
		{
			if (onSingleUpdate == null) return;
			bool isEmpty = _actions.Count <= 0;
			_actions.AddLast(onSingleUpdate);
			if (isEmpty)
			{
				Scripting.Update += ExecuteActions;
			}
		}

		public void AfterFirstUpdate(Action action)
		{
			if (_isAfterFirstUpdate) action?.Invoke();
			else OnUpdate_Once(action);
		}

		private void ExecuteActions()
		{
			foreach (var action in _actions)
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

			_actions.Clear();

			// TODO: Multithreaded code = race condition
			Scripting.Update -= ExecuteActions;
		}
	}
}