using System;
using System.Runtime.CompilerServices;
using FlaxEngine;
using Newtonsoft.Json;

namespace NodeGraphs
{
    /// <summary>
    /// A generic graph node that can execute an action
    /// </summary>
    /// <remarks>
    /// https://github.com/FlaxEngine/FlaxAPI/blob/45537b251b8f88987456b5bc5daf107005c6ec33/FlaxEngine/API/BinaryAssets/AnimationGraph.cs#L24
    /// </remarks>
    public abstract class GraphNode<TContext> where TContext : GraphContext
    {
        [Serialize]
        public readonly NodeDefinition Definition;

        protected GraphNode(NodeDefinition definition)
        {
            Definition = definition;
        }

        [NoSerialize]
        public TContext Context { get; internal set; }

        /// <summary>
        /// Initialize
        /// </summary>
        public virtual void OnEnable()
        {
        }

        /// <summary>
        /// Update
        /// </summary>
        public virtual void OnUpdate()
        {
        }

        /// <summary>
        /// Cleanup
        /// </summary>
        public virtual void OnDisable()
        {
        }

        protected T CastTo<T>(object value)
        {
            if (value == null) return default(T);
            if (typeof(T) == typeof(float))
                // Special handling for numbers
                // TODO: Replace this with something more efficient and/or better
                // TODO: Handle the type casting correctly (float --> Vector2)
                return (T) Convert.ChangeType(value, typeof(T));
            if (value is T castedValue) return castedValue;
            return default(T);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasInput(int index)
        {
            return Definition.InputIndices[index] != -1;
        }

        public object GetInput(int index)
        {
            return Context.Variables[Definition.InputIndices[index]];
        }

        public T GetInput<T>(int index)
        {
            return CastTo<T>(GetInput(index));
        }

        public T GetInputOrDefault<T>(int index, T defaultValue)
        {
            return HasInput(index) ? CastTo<T>(GetInput(index)) : defaultValue;
        }

        public void Return(int index, object returnValue)
        {
            int outputIndex = Definition.OutputIndices[index];
            if(outputIndex != -1) Context.Variables[outputIndex] = returnValue;
        }

        public struct NodeDefinition
        {
            public int GroupId;
            public int TypeId;
            public object[] Values;

            public int[] InputIndices;
            public int[] OutputIndices;
        }
    }
}