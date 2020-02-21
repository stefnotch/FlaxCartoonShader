using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEditor.Viewport.Previews;
using FlaxEngine;
using FlaxEngine.GUI;

namespace RenderingGraph.Editor
{
    public class RenderingGraphPreview : AssetPreview
    {
        private RenderingGraph _renderingGraph;

        // Preview will be expanded later
        public RenderingGraphPreview(bool useWidgets) : base(useWidgets)
        {
            Task.Enabled = false;
        }

        public RenderingGraph RenderingGraph
        {
            get => _renderingGraph;
            set => _renderingGraph = value;
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            // Manually update simulation
            RenderingGraph?.Update(deltaTime);
        }

        /// <inheritdoc />
        public override void Draw()
        {
            base.Draw();

            if (RenderingGraph == null) return;

            Render2D.DrawTexture(RenderingGraph.Output, new Rectangle(Vector2.Zero, Size), Color.White);
        }
    }
}
