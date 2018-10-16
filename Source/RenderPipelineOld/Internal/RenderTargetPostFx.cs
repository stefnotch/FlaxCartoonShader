using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CartoonShader.Source.MeshGenerators;
using FlaxEngine;
using FlaxEngine.Rendering;

namespace CartoonShader.Source.RenderPipelineOld
{
	public class RenderTargetPostFx : IDisposable
	{
		private Camera _orthographicCamera;
		private ModelActor _modelActor;
		private Model _model;
		private const float ZPos = 100f;
		private RenderTarget _output;
		private SceneRenderTask _task; //TODO: Add PostFx
		private Vector2 _size;

		[Serialize]
		[ShowInEditor]
		public readonly RenderTargetToMaterial RenderTargetToMaterial = new RenderTargetToMaterial();

		/// <summary>
		/// Output RenderTarget
		/// </summary>
		public RenderTarget Output { get => _output; private set => _output = value; }

		/// <summary>
		/// Output RenderTarget Size
		/// </summary>
		public Vector2 Size
		{
			get => _size;
			set
			{
				if (_size != value)
				{
					_size = value; //TODO: Stuffz here
					if (_modelActor)
					{
						_modelActor.LocalPosition = new Vector3(_size * -0.5f, ZPos);
					}
					if (_model)
					{
						UpdateMesh(_model.LODs[0].Meshes[0]);
					}
					if (_output)
					{
						_output.Init(PixelFormat.R8G8B8A8_UNorm, _size);
					}
				}
			}
		}

		/// <summary>
		/// Should a quad be generated for every pixel
		/// </summary>
		public bool QuadForEachPixel = false;

		public bool HalfPixelOffset = false;

		/// <summary>
		/// Rendering order
		/// TODO: Be able to set up a proper pipeline where the order gets decided upon automagically
		/// </summary>
		public int Order = -100000;

		/// <summary>
		/// If the RenderTask is enabled
		/// </summary>
		public bool IsStarted
		{
			get => _task.Enabled;
		}

		public RenderTargetPostFx()
		{
		}

		public void Initialize(Vector2 screenSize)
		{
			_orthographicCamera = CreateOrthographicCamera();
			_model = Content.CreateVirtualAsset<Model>();
			_model.SetupLODs(1);
			_modelActor = FlaxEngine.Object.New<ModelActor>();
			_modelActor.Model = _model;
			RenderTargetToMaterial.ModelActor = _modelActor;

			//SceneRenderer.cs
			if (!_output) _output = RenderTarget.New();

			Size = screenSize;

			if (_task) throw new Exception("Scene Render Task is not null");
			// Create rendering task
			_task = CreateRenderTask();

			RenderTargetToMaterial.Initialize();
		}

		public void StartRenderTask()
		{
			//TODO: Check if we can already enable the task
			_task.Enabled = true;

			RenderTargetToMaterial.StartRenderTask();
		}

		private void UpdateMesh(Mesh mesh)
		{
			if (mesh == null) return;

			int width = (int)_size.X;
			int height = (int)_size.Y;

			if (QuadForEachPixel)
			{
				new ScreenPixelQuadsGenerator(new Int2(width, height)).Generate(mesh);
			}
			else
			{
				new QuadGenerator(_size).Generate(mesh);
			}
		}

		private SceneRenderTask CreateRenderTask()
		{
			SceneRenderTask task = RenderTask.Create<SceneRenderTask>();
			task.Order = Order;
			task.Camera = _orthographicCamera;
			task.Output = _output;
			task.Enabled = false;
			task.AllowGlobalCustomPostFx = false;
			task.CustomActors.Add(_orthographicCamera);
			task.CustomActors.Add(_modelActor);
			task.ActorsSource = ActorsSources.CustomActors;
			task.Mode = ViewMode.Emissive;

			return task;
		}

		private Camera CreateOrthographicCamera()
		{
			_orthographicCamera = FlaxEngine.Object.New<Camera>();
			_orthographicCamera.UsePerspective = false;
			_orthographicCamera.NearPlane = 2;
			_orthographicCamera.FarPlane = 1000;
			_orthographicCamera.OrthographicScale = 1;
			_orthographicCamera.LocalPosition = Vector3.Zero;
			return _orthographicCamera;
		}

		#region IDisposable Support

		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					FlaxEngine.Object.Destroy(ref _orthographicCamera);
					FlaxEngine.Object.Destroy(ref _model);
					FlaxEngine.Object.Destroy(ref _modelActor);
					FlaxEngine.Object.Destroy(ref _task);
					if (_output)
					{
						FlaxEngine.Object.Destroy(ref _output);
					}
					RenderTargetToMaterial.Dispose();
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
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