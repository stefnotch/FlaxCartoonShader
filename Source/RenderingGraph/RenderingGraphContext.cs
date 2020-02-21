using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;
using NodeGraphs;

namespace RenderingGraph
{
    public class RenderingGraphContext : GraphContext
    {
        public GPUContext GPUContext;
        public RenderTask RenderTask;
    }
}
