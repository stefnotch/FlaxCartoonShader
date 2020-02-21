using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;
using RenderingGraph;

namespace CartoonShader.Source
{
    public class TestNewRenderers : Script
    {
        private readonly List<FlaxEngine.Object> _toDisposeFlax = new List<FlaxEngine.Object>();
        private readonly List<IDisposable> _toDisposeIDisposable = new List<IDisposable>();

        public Camera Camera { get; set; }
        public MaterialBase EffectMaterial { get; set; }
        public MaterialBase PixelsEffectMaterial { get; set; }
        public MaterialBase OutputMaterial { get; set; }

        public override void OnEnable()
        {
            OutputMaterial.WaitForLoaded();
            EffectMaterial.WaitForLoaded();
            PixelsEffectMaterial.WaitForLoaded();

            var effectMaterial = DisposeLater(EffectMaterial.CreateVirtualInstance());
            var outputMaterial = DisposeLater(OutputMaterial.CreateVirtualInstance());
            var pixelsMaterial = DisposeLater(PixelsEffectMaterial.CreateVirtualInstance());

            effectMaterial.WaitForLoaded();
            outputMaterial.WaitForLoaded();
            pixelsMaterial.WaitForLoaded();

            Func<Int2> screenSizeGetter = () => new Int2(Mathf.FloorToInt(Screen.Size.X), Mathf.FloorToInt(Screen.Size.Y));

            // var cameraRenderer = DisposeLater(new CameraRenderer(Camera, screenSizeGetter), 0);

            //var effectRenderer = DisposeLater(new PostFxRenderer(effectMaterial, screenSizeGetter), 0);
            //  effectRenderer.SetInput(cameraRenderer.Output);

            var pixelsRenderer = DisposeLater(new PixelsRenderer(pixelsMaterial, screenSizeGetter), 0);
           // pixelsRenderer.SetInput("Image", effectRenderer.Output);

            // TODO: 
            pixelsRenderer.Output.ContinueWith(output =>
            {
                // Add materials
                ModelEntryInfo[] entries = GetEntries(Actor);

                for (int i = 0; i < entries.Length; i++)
                {
                    if (!entries[i].Material)
                    {
                        entries[i].Material = outputMaterial;
                    }
                }

                SetAsMaterialInputs(entries, output.Result);
            });
        }

        //https://codeblog.jonskeet.uk/2010/11/02/evil-code-overload-resolution-workaround/
        public T DisposeLater<T>(T obj) where T : FlaxEngine.Object
        {
            _toDisposeFlax.Add(obj);
            return obj;
        }

        public U DisposeLater<U>(U obj, int dummy = 0) where U : IDisposable
        {
            _toDisposeIDisposable.Add(obj);
            return obj;
        }

        public static void SetAsMaterialInputs(ModelEntryInfo[] entries, GPUTexture texture)
        {
            if (texture == null) return;

            for (int i = 0; i < entries.Length; i++)
            {
                var material = entries[i].Material;
                if (!material) continue;

                material.WaitForLoaded();
                material.GetParam("Image").Value = texture;
            }
        }

        private static ModelEntryInfo[] GetEntries(Actor actor)
        {
            ModelEntryInfo[] entries = null;
            if (actor is StaticModel staticModel)
            {
                entries = staticModel.Entries;
            }

            if (actor is AnimatedModel animatedModel)
            {
                entries = animatedModel.Entries;
            }

            return entries;
        }

        public override void OnDisable()
        {
            for (int i = 0; i < _toDisposeIDisposable.Count; i++)
            {
                _toDisposeIDisposable[i].Dispose();
            }
            _toDisposeIDisposable.Clear();

            for (int i = 0; i < _toDisposeFlax.Count; i++)
            {
                Destroy(_toDisposeFlax[i]);
            }
            _toDisposeFlax.Clear();
        }
    }
}
