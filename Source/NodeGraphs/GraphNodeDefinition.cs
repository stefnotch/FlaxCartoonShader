using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeGraphs
{
    public struct GraphNodeDefinition
    {
        public int GroupId;
        public int TypeId;
        public int Index;
        public object[] Values;

        public int[] InputIndices;
        public int[] OutputIndices;
    }
}
