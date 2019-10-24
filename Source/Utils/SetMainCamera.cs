using System;
using System.Collections.Generic;
using FlaxEngine;

namespace CartoonShader.Source
{
	public class SetMainCamera : Script
	{
		private Camera _previousMain;
		public Camera Main;

		private void OnEnable()
		{
			_previousMain = Camera.MainCamera;
			Camera.MainCamera = Main;
		}

		private void DisableEnable()
		{
			if (_previousMain) Camera.MainCamera = _previousMain;
		}
	}
}