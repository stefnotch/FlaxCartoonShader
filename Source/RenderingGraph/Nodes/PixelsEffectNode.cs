using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlaxEngine;
using FlaxEngine.Utilities;
using NodeGraphs;

namespace RenderingGraph.Nodes
{
    public class PixelsEffectNode : RenderingNode<SceneRenderTask>
    {
        protected GPUTexture Output;
        protected Vector2 Size => Vector2.Max(GetInputOrDefault<Vector2>(0, Context.Size), Vector2.One);

        private MaterialParameter[] _inputParameters;
        private MaterialBase _material;
        private MaterialInstance _materialInstance;

        // TODO: It's probably a good idea to store a proper reference to the material
        // public MaterialBase Material;

        private const float DistanceFromOrigin = 100f;
        private Vector2 _cachedSize;
        private Camera _orthographicCamera;
        private StaticModel _modelActor;
        private Model _model;

        public PixelsEffectNode(GraphNodeDefinition definition) : base(definition)
        {

        }

        public override void OnEnable()
        {
            base.OnEnable();
            _material = Content.Load<MaterialBase>(ParseGuid(Definition.Values[0]));
            if (!_material || !_material.IsSurface) return;
            _materialInstance = _material.CreateVirtualInstance();

            _inputParameters = GetPublicParameters(_materialInstance);

            var size = Size;
            _orthographicCamera = CreateOrthographicCamera();
            _model = Content.CreateVirtualAsset<Model>();
            _model.SetupLODs(1);
            _model.SetupMaterialSlots(1);
            _model.MinScreenSize = 0;
            // TODO: Optimize, use instanced rendering and whatnot
            GenerateGridMesh(_model.LODs[0].Meshes[0], size);
            _cachedSize = size;
            _modelActor = FlaxEngine.Object.New<StaticModel>();
            _modelActor.Model = _model;
            _modelActor.DrawModes = DrawPass.Depth | DrawPass.GBuffer; // TODO: Optionally enable transparency?
            _modelActor.Entries[0].ReceiveDecals = false;
            _modelActor.Entries[0].ShadowsMode = ShadowsCastingMode.None;
            _modelActor.Entries[0].Material = _materialInstance;
            _modelActor.LocalPosition = new Vector3(size * -0.5f, DistanceFromOrigin);

            Output = CreateOutputTexture(Size);

            RenderTask.AllowGlobalCustomPostFx = false;
            RenderTask.Order = Order;
            RenderTask.Camera = _orthographicCamera;
            RenderTask.ActorsSource = ActorsSources.CustomActors;
            RenderTask.Output = Output;
            RenderTask.Begin += OnRenderTaskBegin;
            RenderTask.End += OnRenderTaskEnd;
            RenderTask.CustomActors.Add(_modelActor);
            RenderTask.View.MaxShadowsQuality = Quality.Low;
            RenderTask.View.Mode = ViewMode.Emissive;
            RenderTask.View.Flags = ViewFlags.None;
        }

        public void OnRenderTaskBegin(SceneRenderTask task, GPUContext context)
        {
            var size = Size;
            _modelActor.LocalPosition = new Vector3(size.X * -0.5f, size.Y * -0.5f, DistanceFromOrigin);
            if (_cachedSize != size)
            {
                GenerateGridMesh(_model.LODs[0].Meshes[0], size);
                _cachedSize = size;
            }

            if (!_materialInstance) return;

            for (int i = 0; i < _inputParameters.Length; i++)
            {
                if (!HasInput(i + 1)) continue;
                _inputParameters[i].Value = GetInput(i + 1);
            }
        }

        public void OnRenderTaskEnd(SceneRenderTask task, GPUContext context)
        {
            Return(0, Output);
        }

        public override void OnDisable()
        {
            if (RenderTask)
            {
                RenderTask.Begin -= OnRenderTaskBegin;
                RenderTask.End -= OnRenderTaskEnd;
                RenderTask.Dispose();
            }

            FlaxEngine.Object.Destroy(ref RenderTask);
            _inputParameters = null;
            FlaxEngine.Object.Destroy(ref _orthographicCamera);
            FlaxEngine.Object.Destroy(ref _modelActor);
            FlaxEngine.Object.Destroy(ref _model);
            FlaxEngine.Object.Destroy(ref _materialInstance);
            FlaxEngine.Object.Destroy(ref _material);
            FlaxEngine.Object.Destroy(ref Output);
            base.OnDisable();
        }

        private static Camera CreateOrthographicCamera()
        {
            var orthographicCamera = FlaxEngine.Object.New<Camera>();
            orthographicCamera.NearPlane = 2;
            orthographicCamera.FarPlane = 1000;
            orthographicCamera.OrthographicScale = 1;
            orthographicCamera.LocalPosition = Vector3.Zero;
            orthographicCamera.UsePerspective = false;
            return orthographicCamera;
        }

        private static void GenerateGridMesh(Mesh mesh, Vector2 size)
        {
            if (mesh == null) return;

            int width = Math.Max((int)size.X, 1);
            int height = Math.Max((int)size.Y, 1);

            var vertices = new Vector3[width * height * 4];
            var uvs = new Vector2[width * height * 4];
            var triangles = new int[width * height * 6];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int index = (y + x * height) * 4;

                    vertices[index] = new Vector3(x, y, 0);
                    vertices[index + 1] = new Vector3(x + 1, y, 0);
                    vertices[index + 2] = new Vector3(x + 1, y + 1, 0);
                    vertices[index + 3] = new Vector3(x, y + 1, 0);

                    // Half pixel offset so that we always sample the center of the pixels
                    Vector2 uv = new Vector2(
                        (x + 0.5f) / (float)width,
                        1f - (y + 0.5f) / (float)height
                    );

                    for (int i = 0; i < 4; i++)
                    {
                        uvs[index + i] = uv;
                    }

                    int triangleIndex = (y + x * height) * 6;
                    triangles[triangleIndex] = index;
                    triangles[triangleIndex + 1] = index + 3;
                    triangles[triangleIndex + 2] = index + 1;

                    triangles[triangleIndex + 3] = index + 1;
                    triangles[triangleIndex + 4] = index + 3;
                    triangles[triangleIndex + 5] = index + 2;
                }
            }

            mesh.UpdateMesh(vertices, triangles, uv: uvs);
        }
    }
}
