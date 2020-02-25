using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEditor.Content;
using FlaxEditor.Surface;
using FlaxEditor.Windows.Assets;
using FlaxEngine;

namespace RenderingGraph.Editor
{
    public class RenderingGraphWindow : VisjectSurfaceWindow<JsonAsset, RenderingGraphSurface, RenderingGraphPreview>
    {
        /// <summary>
        /// The allowed parameter types. Define custom list based on ParameterType enum.
        /// </summary>
        private enum NewParameterType
        {
            Float = ParameterType.Float,
            Vector2 = ParameterType.Vector2,
            Vector3 = ParameterType.Vector3,
            Vector4 = ParameterType.Vector4,
            Color = ParameterType.Color,
            Texture = ParameterType.Texture
        }

        /// <summary>
        /// The properties proxy object.
        /// </summary>
        private sealed class PropertiesProxy
        {
            [EditorOrder(1000), EditorDisplay("Parameters"), CustomEditor(typeof(ParametersEditor)), NoSerialize]

            public RenderingGraphWindow Window { get; set; }

            [EditorOrder(20), EditorDisplay("General"), Tooltip("It's for demo purposes")]
            public int DemoInteger { get; set; }

            [HideInEditor, Serialize]
            public List<SurfaceParameter> Parameters
            {
                get => Window.Surface.Parameters;
                set => throw new Exception("No setter.");
            }

            /// <summary>
            /// Gathers parameters from the specified window.
            /// </summary>
            /// <param name="window">The window.</param>
            public void OnLoad(RenderingGraphWindow window)
            {
                // Link
                Window = window;
            }

            /// <summary>
            /// Clears temporary data.
            /// </summary>
            public void OnClean()
            {
                // Unlink
                Window = null;
            }
        }

        private readonly PropertiesProxy _properties;

        private RenderingGraph _assetInstance;

        /// <inheritdoc />
        public RenderingGraphWindow(FlaxEditor.Editor editor, AssetItem item)
        : base(editor, item)
        {
            NewParameterTypes = typeof(NewParameterType);

            // Asset preview
            _preview = new RenderingGraphPreview(true)
            {
                Parent = _split2.Panel1
            };

            // Asset properties proxy
            _properties = new PropertiesProxy();
            _propertiesEditor.Select(_properties);

            // Surface
            _surface = new RenderingGraphSurface(this, Save, _undo)
            {
                Parent = _split1.Panel1,
                Enabled = false
            };

            // Toolstrip
            _toolstrip.AddSeparator();
            _toolstrip.AddButton(editor.Icons.BracketsSlash32, () => ShowJson(_asset)).LinkTooltip("Show asset contents");
        }

        /// <summary>
        /// Shows the JSON contents window.
        /// </summary>
        /// <param name="asset">The JSON asset.</param>
        public static void ShowJson(JsonAsset asset)
        {
            FlaxEditor.Utilities.Utils.ShowSourceCode(asset.Data, "Asset JSON");
        }

        /// <inheritdoc />
        protected override void UnlinkItem()
        {
            // Cleanup
            _properties.OnClean();
            _preview.RenderingGraph = null;
            _assetInstance.OnDestroy();

            base.UnlinkItem();
        }

        /// <inheritdoc />
        protected override void OnAssetLinked()
        {
            // Setup
            _assetInstance = _asset.CreateInstance<RenderingGraph>();
            _preview.RenderingGraph = _assetInstance;
            _preview.RenderingGraph.Enabled = true;

            base.OnAssetLinked();
        }

        /// <inheritdoc />
        public override string SurfaceName => "Rendering Graph";

        /// <inheritdoc />
        public override byte[] SurfaceData
        {
            get => RenderingGraphSurface.LoadSurface(_asset, _assetInstance, true);
            set
            {
                // Save data to the temporary asset
                if (RenderingGraphSurface.SaveSurface(_asset, _assetInstance, value))
                {
                    // Error
                    _surface.MarkAsEdited();
                    FlaxEditor.Editor.LogError("Failed to save surface data");
                }
                // Optionally reset the preview
            }
        }

        /// <inheritdoc />
        protected override bool LoadSurface()
        {

            // Load surface data from the asset
            byte[] data = RenderingGraphSurface.LoadSurface(_asset, _assetInstance, true);
            if (data == null)
            {
                // Error
                FlaxEditor.Editor.LogError("Failed to load rendering graph surface data.");
                return true;
            }

            // Load surface graph
            if (_surface.Load(data))
            {
                // Error
                FlaxEditor.Editor.LogError("Failed to load rendering graph surface.");
                return true;
            }

            // Init asset properties and parameters proxy
            _properties.OnLoad(this);

            return false;
        }

        /// <inheritdoc />
        protected override bool SaveSurface()
        {
            try
            {
                bool enabled = _assetInstance.Enabled;
                _assetInstance.Enabled = false;
                _surface.CompileSurface(_assetInstance);
                _surface.Save();
                _assetInstance.Enabled = enabled;
                _surface.Save();

            }
            catch (Exception e)
            {
                FlaxEditor.Editor.LogWarning(e);
                return true;
            }


            return false;
        }

        /// <inheritdoc />
        protected override void SetParameter(int index, object value)
        {
            _assetInstance.Parameters[index].Value = value;

            base.SetParameter(index, value);
        }
    }
}
