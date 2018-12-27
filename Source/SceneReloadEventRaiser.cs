using System;
using System.Collections.Generic;
using FlaxEngine;

namespace CartoonShader
{
	public class SceneReloadEventRaiser : Script
	{
		private static SceneReloadEventRaiser _instance;

		public event Action SceneReload;

		[HideInEditor]
		[NoSerialize]
		public static SceneReloadEventRaiser Instance
		{
			get
			{
				if (_instance == null)
				{
					var actor = New<EmptyActor>();
					actor.HideFlags = HideFlags.FullyHidden;
					_instance = actor.AddScript<SceneReloadEventRaiser>();
					SceneManager.SpawnActor(actor);
				}
				return _instance;
			}
		}

		private void OnDestroy()
		{
			SceneReload?.Invoke();
		}
	}
}