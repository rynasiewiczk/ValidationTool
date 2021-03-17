namespace LazySloth.Validation
{
    using System.Reflection;
    using UnityEngine;

    public class FieldInstanceData
    {
        /// <summary>
        /// Source object
        /// </summary>
        public readonly object Obj;
        
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

        public FieldInstanceData(object obj, FieldInfo fieldInfo, Component component, object instance)
        {
            Obj = obj;
            FieldInfo = fieldInfo;
            Component = component;
            Instance = instance;
        }

    }
}