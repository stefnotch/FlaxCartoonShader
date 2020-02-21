using System;
using System.Collections.Generic;
using FlaxEditor;
using FlaxEditor.Content;
using FlaxEditor.Windows;
using FlaxEngine;
using FlaxEngine.Collections;

namespace RenderingGraph.Editor
{
    public class RenderingGraphProxy : JsonAssetProxy
    {
        /// <inheritdoc />
        public override string Name => "Rendering Graph";

        /// <inheritdoc />
        public override EditorWindow Open(FlaxEditor.Editor editor, ContentItem item)
        {
            return new RenderingGraphWindow(editor, (JsonAssetItem)item);
        }

        /// <inheritdoc />
        public override Color AccentColor => Color.FromRGB(0x0F0371);

        /// <inheritdoc />
        public override string TypeName { get; } = typeof(RenderingGraph).FullName;

        /// <inheritdoc />
        public override bool CanCreate(ContentFolder targetLocation)
        {
            return targetLocation.CanHaveAssets;
        }

        public override bool IsProxyFor(ContentItem item)
        {
            FlaxEngine.Content.GetAssetInfo(item.Path, out string typeName, out Guid id);
            return TypeName == typeName && base.IsProxyFor(item);
        }

        /// <inheritdoc />
        public override void Create(string outputPath, object arg)
        {
            FlaxEditor.Editor.SaveJsonAsset(outputPath, new RenderingGraph());
        }
    }
}
