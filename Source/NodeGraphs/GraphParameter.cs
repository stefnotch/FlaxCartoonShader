namespace NodeGraphs
{
    /// <summary>
    /// A graph parameter
    /// </summary>
    public class GraphParameter<TContext> where TContext : GraphContext
    {
        public string Name;
        public int OutputIndex;
        public object Value;

        public GraphParameter(string name, object value, int outputIndex)
        {
            Name = name;
            Value = value;
            OutputIndex = outputIndex;
        }

        public void Update(TContext context)
        {
            context.Variables[OutputIndex] = Value;
        }
    }
}