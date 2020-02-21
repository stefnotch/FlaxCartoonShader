using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlaxEngine;
using FlaxEngine.Utilities;

namespace RenderingGraph.Nodes
{
    /// <summary>
    /// Renders a camera or the main camera
    /// </summary>
    /// <remarks>
    /// Make sure to keep this in sync with https://github.com/FlaxEngine/FlaxAPI/blob/master/FlaxEngine/Rendering/SceneRenderTask.cs
    /// </remarks>
    public class CameraNode : OutputNode
    {
        private ActorsSources _actorsSources = ActorsSources.Scenes;
        private readonly List<Actor> _customActors = new List<Actor>();
        private RenderBuffers _buffers;
        private RenderView _view;
        private readonly HashSet<PostProcessEffect> _customPostFx = new HashSet<PostProcessEffect>();
        private bool _allowGlobalCustomPostFx = true;
        private readonly HashSet<PostProcessEffect> _postFx = new HashSet<PostProcessEffect>();

        protected Camera Camera => GetInputOrDefault<Camera>(1, Camera.MainCamera);

        public CameraNode(NodeDefinition definition) : base(definition)
        {
        }

        public override void OnEnable()
        {
            base.OnEnable();
            _view.Init();
        }

        public override void OnDisable()
        {
            _customActors.Clear();
            _customPostFx.Clear();
            base.OnDisable();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (!_buffers) _buffers = RenderBuffers.New();
            _buffers.Size = Output.Size;
            var viewport = new Viewport(Vector2.Zero, _buffers.Size);
            var camera = Camera;
            if (camera)
            {
                _view.CopyFrom(camera, ref viewport);
            }

            _postFx.Clear();
            _postFx.AddRange(_customPostFx);
            if (_allowGlobalCustomPostFx)
            {
                _postFx.AddRange(SceneRenderTask.GlobalCustomPostFx);
            }

            if (camera)
            {
                _postFx.AddRange( Camera.GetScripts<PostProcessEffect>());
            }

            Context.GPUContext.DrawScene(Context.RenderTask, Output, _buffers, ref _view, _customActors, _actorsSources, _postFx);

            Return(0, Output);
            Return(1, _buffers.DepthBuffer);
            Return(2, _buffers.MotionVectors);
        }
    }
}
