using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;

namespace CartoonShader.Source.RenderPipelineOld
{
	public class ScriptUtils
	{
		private static ScriptUtils _instance;

		private ScriptUtils()
		{
		}

		public static ScriptUtils Instance
		{
			get
			{
				if (_instance == null) _instance = new ScriptUtils();
				return _instance;
			}
		}

		private readonly LinkedList<Action> _actions = new LinkedList<Action>();

		public void AddSingleUpdate(Action onSingleUpdate)
		{
			if (onSingleUpdate == null) return;
			bool isEmpty = _actions.Count <= 0;
			_actions.AddLast(onSingleUpdate);
			if (isEmpty)
			{
				Scripting.Update += ExecuteActions;
			}
		}

		private void ExecuteActions()
		{
			foreach (var action in _actions)
			{
				// TODO: Don't let 1 exception affect the rest
				action.Invoke();
			}

			_actions.Clear();

			// TODO: Multithreaded code = race condition
			Scripting.Update -= ExecuteActions;
		}
	}
}