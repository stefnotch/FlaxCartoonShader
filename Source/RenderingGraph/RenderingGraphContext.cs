﻿using System;
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
        public Vector2 Size = Vector2.One;
        public int NodesCount;
        public Action<int> ExecutePreviousNodes;
        public GPUContext GPUContext;
    }
}
