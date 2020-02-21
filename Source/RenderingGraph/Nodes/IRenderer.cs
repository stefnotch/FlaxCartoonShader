using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;

namespace RenderingGraph
{
    public interface IRenderer : IDisposable
    {
        string Name { get; set; }

        // Note:
        // The SceneRenderTask has some RenderBuffers that get created at runtime
        // And those RenderBuffers contain the MotionVectors GPUTexture. 
        // Either create the RenderBuffers up front or keep this Task<> design.
        Task<GPUTexture> Output { get; }
    }
}
