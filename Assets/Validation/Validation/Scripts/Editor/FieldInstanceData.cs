namespace LazySloth.Validation
{
    using System.Collections.Generic;
    using System.Linq;
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

        public IReadOnlyList<object> Stack = new List<object>();

        public FieldInstanceData(Component component, MemberInfoWithValue fieldData)
        {
            Obj = fieldData.Object;
            FieldInfo = fieldData.MemberInfo as FieldInfo;
            Component = component;
            Instance = fieldData.Value;
            Stack = fieldData.Stack;
        }

        public FieldInstanceData(object obj, FieldInfo fieldInfo, Component component, object instance, List<object> stack)
        {
            Obj = obj;
            FieldInfo = fieldInfo;
            Component = component;
            Instance = instance;
            
            Stack = stack;
        }

        public string GetStackLog()
        {
            var result = "";
            foreach (var s in Stack)
            {
                result += s.ToString();

                if (Stack.Last() != s)
                {
                    result += "<color=red>/</color>";
                }
            }

            return result;
        }
    }
}