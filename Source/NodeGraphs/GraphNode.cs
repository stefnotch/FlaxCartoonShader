﻿using System;
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
    public abstract class GraphNode
    {
        public int[] InputIndices;
        public int[] OutputIndices;

        [NoSerialize]
        public object[] Variables { get; internal set; }

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
                return (T)Convert.ChangeType(value, typeof(T));
            if (value is T castedValue) return castedValue;
            return default(T);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasInput(int index)
        {
            return InputIndices[index] != -1;
        }

        public object GetInput(int index)
        {
            return Variables[InputIndices[index]];
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
            int outputIndex = OutputIndices[index];
            if (outputIndex != -1) Variables[outputIndex] = returnValue;
        }
    }
}