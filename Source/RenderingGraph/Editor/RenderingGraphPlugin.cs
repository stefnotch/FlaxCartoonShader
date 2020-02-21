using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEditor;
using FlaxEngine;

namespace RenderingGraph.Editor
{
    public class RenderingGraphPlugin : EditorPlugin
    {
        private RenderingGraphProxy _renderingGraphProxy;

        /// <inheritdoc />
        public override void InitializeEditor()
        {
            base.InitializeEditor();
            _renderingGraphProxy = new RenderingGraphProxy();
            Editor.ContentDatabase.Proxy.Insert(22, _renderingGraphProxy);
        }

        /// <inheritdoc />
        public override void Deinitialize()
        {
            Editor.ContentDatabase.Proxy.Remove(_renderingGraphProxy);
            base.Deinitialize();
        }
    }
}
