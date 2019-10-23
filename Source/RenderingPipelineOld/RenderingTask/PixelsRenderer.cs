using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CartoonShader.Source.RenderingPipeline.RenderingOutput;
using FlaxEngine;
using FlaxEngine.Rendering;

namespace CartoonShader.Source.RenderingPipeline.RenderingTask
{
	/// <summary>
	/// For the effects where you need to move individual pixels
	/// </summary>
	public class PixelsRenderer : IRenderer
	{
		private const float ZPos = 100f;
		private Camera _orthographicCamera;
		private StaticModel _modelActor;
		private Model _model;
		private Vector2 _size;

		// TODO: The user should pass in a materialinstance and dispose of it himself?
		public PixelsRenderer(MaterialBase material, Vector2 size, string name = "")
		{
			if (!material.IsSurface) throw new ArgumentException("Surface Material expected", nameof(material));
			Name = name;
			Inputs = new MaterialRenderInputs(material);

			_size = size;
			Output = new RenderOutput(size, this);

			if (!Task) Task = RenderTask.Create<SceneRenderTask>();
			Task.Enabled = false;
			// TODO: Enable the task!
			Task.Output = Output.RenderTarget;

			if (!_model)
			{
				_model = Content.CreateVirtualAsset<Model>();
				_model.SetupLODs(1);
				_model.SetupMaterialSlots(1);
				_model.MaterialSlots[0].ShadowsMode = ShadowsCastingMode.None;
			}
			if (!_modelActor)
			{
				_modelActor = FlaxEngine.Object.New<StaticModel>();
				_modelActor.Model = _model;
				_modelActor.Entries[0].ReceiveDecals = false;
				_modelActor.Entries[0].ShadowsMode = ShadowsCastingMode.None;
				Task.CustomActors.Add(_modelActor);
			}

			if (!_orthographicCamera)
			{
				_orthographicCamera = CreateOrthographicCamera();

				Task.Camera = _orthographicCamera;
			}

			SceneManager.SpawnActor(_orthographicCamera, SceneManager.FindActor("SpawnHere"));
			SceneManager.SpawnActor(_modelActor, SceneManager.FindActor("SpawnHere"));

			SizeChanged(_size);

			Task.AllowGlobalCustomPostFx = false;
			Task.ActorsSource = ActorsSources.CustomActors;
			Task.Mode = ViewMode.Emissive; // Appawently this doesn't work with Additive mode
			Task.Flags = ViewFlags.None;

			Task.Begin += OnRenderTaskBegin;

			Scripting.InvokeOnUpdate(() =>
			{
				if (_modelActor) _modelActor.Entries[0].Material = Material;
			});
		}

		public string Name { get; set; }

		public MaterialRenderInputs Inputs { get; }

		public SceneRenderTask Task { get; }

		RenderTask IRenderer.Task => Task;

		public RenderOutput Output { get; }

		public MaterialBase Material => Inputs.Material;

		public PixelsRenderer SetInput(string name, RenderOutput renderOutput)
		{
			Inputs[name] = renderOutput;
			return this;
		}

		private void OnRenderTaskBegin(SceneRenderTask task, GPUContext context)
		{
			if (Output.Size != _size)
			{
				_size = Output.Size;
				SizeChanged(_size);
			}
			//context.DrawScene()
		}

		private void SizeChanged(Vector2 size)
		{
			if (_modelActor)
			{
				_modelActor.LocalPosition = new Vector3(size * -0.5f, ZPos);
			}
			if (_model)
			{
				UpdateMesh(_model.LODs[0].Meshes[0]);
			}
		}

		private void UpdateMesh(Mesh mesh)
		{
			if (mesh == null) return;

			// TODO: This can cause issues if it doesn't perfectly line up with the actual screen pixels
			int width = Mathf.CeilToInt(_size.X);
			int height = Mathf.CeilToInt(_size.Y);

			// TODO: Use instanced rendering https://learnopengl.com/Advanced-OpenGL/Instancing
			new MeshGenerators.ScreenPixelQuadsGenerator(new Int2(width, height)).Generate(mesh);
		}

		private Camera CreateOrthographicCamera()
		{
			_orthographicCamera = FlaxEngine.Object.New<Camera>();
			_orthographicCamera.NearPlane = 2;
			_orthographicCamera.FarPlane = 1000;
			_orthographicCamera.OrthographicScale = 1;
			_orthographicCamera.LocalPosition = Vector3.Zero;
			_orthographicCamera.UsePerspective = false;
			return _orthographicCamera;
		}

		#region IDisposable Support

		private bool _disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				if (disposing)
				{
					if (Task)
					{
						Task.Begin -= OnRenderTaskBegin;
						Task.Enabled = false;
					}
					Task?.Dispose();
					FlaxEngine.Object.Destroy(Task);
					FlaxEngine.Object.Destroy(Output.RenderTarget);
					FlaxEngine.Object.Destroy(_orthographicCamera);
					FlaxEngine.Object.Destroy(_modelActor);
					FlaxEngine.Object.Destroy(_model);
					Inputs.Material = null;
				}
				_disposedValue = true;
			}
		}

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
		}

		#endregion IDisposable Support
	}
}