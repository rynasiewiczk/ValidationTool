namespace LazySloth.Validation
{
    using System.Reflection;
    using UnityEngine;

    public class FieldInstanceData
    {
        public readonly object Object;
        
        /// <summary>
        /// Metadata about the field instance
        /// </summary>
        public readonly FieldInfo FieldInfo;

        /// <summary>
        /// Component of the field
        /// </summary>
        public readonly Component Component;

        /// <summary>
        /// Instance of the field
        /// </summary>
        public readonly object Instance;

        public FieldInstanceData(object @object, FieldInfo fieldInfo, Component component, object instance)
        {
            Object = @object;
            FieldInfo = fieldInfo;
            Component = component;
            Instance = instance;
        }

    }
}