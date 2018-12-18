using System;
using System.Collections.Generic;
using CartoonShader.Source.MeshGenerators;
using FlaxEngine;

namespace CartoonShader.Source
{
	[Tooltip("Expects an orthographic camera")]
	public class FullscreenQuad : Script
	{
		/// <summary>
		/// Material to use for this fullscreen quad
		/// </summary>
		public MaterialBase Material;

		/// <summary>
		/// Should a quad be generated for every pixel
		/// </summary>
		public bool QuadForEachPixel = true;

		private Model _tempModel;
		private StaticModel _childModel;
		private const float ZPos = 100f;
		private const float Scale = 0.5f;

		private Vector2 _screenSize;

		private void Start()
		{
			// Create dynamic model with a single LOD with one mesh
			_tempModel = Content.CreateVirtualAsset<Model>();
			_tempModel.SetupLODs(1);

			_childModel = Actor.TemporaryChild<StaticModel>();
			_childModel.Model = _tempModel;

			_childModel.Entries[0].Material = Material;

			_childModel.LocalPosition = new Vector3(0, 0, ZPos);
		}

		private void Update()
		{
			if (_screenSize != Screen.Size)
			{
				_screenSize = Screen.Size;
				UpdateSquare(_tempModel.LODs[0].Meshes[0]);
				_childModel.LocalPosition = new Vector3(Screen.Size * -0.5f, ZPos);
			}
		}

		private void OnDestroy()
		{
			Destroy(ref _childModel);
			Destroy(ref _tempModel);
		}

		private void UpdateSquare(Mesh mesh)
		{
			if (mesh == null) return;

			int width = (int)_screenSize.X;
			int height = (int)_screenSize.Y;

			if (QuadForEachPixel)
			{
				new ScreenPixelQuadsGenerator(new Int2(width, height)).Generate(mesh);
			}
			else
			{
				new QuadGenerator(new Vector2(width, height)).Generate(mesh);
			}
		}
	}
}