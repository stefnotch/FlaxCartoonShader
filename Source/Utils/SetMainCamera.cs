using System;
using System.Collections.Generic;
using FlaxEngine;

namespace CartoonShader.Source
{
    public class SetMainCamera : Script
    {
        private Camera _previousMain;
        public Camera Main;

        public override void OnEnable()
        {
            _previousMain = Camera.MainCamera;
            Camera.MainCamera = Main;
        }

        public override void OnDisable()
        {
            if (_previousMain) Camera.MainCamera = _previousMain;
        }
    }
}