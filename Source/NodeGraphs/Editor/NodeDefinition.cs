namespace NodeGraphs.Editor
{
    public struct NodeDefinition
    {
        public int GroupId;
        public int TypeId;
        public int Index;
        public object[] Values;

        public int[] InputIndices;
        public int[] OutputIndices;
    }
}
