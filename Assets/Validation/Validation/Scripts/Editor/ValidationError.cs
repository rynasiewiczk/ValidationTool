namespace LazySloth.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEngine;

    public class ValidationError
    {
        public Type ValidationType { get; }
        private readonly string _objectName;
        private readonly string _fieldName;
        private readonly string _message;

        public IReadOnlyList<object> Stack;

        public ValidationError(Type validationType, object obj, IReadOnlyList<object> stack, MemberInfo memberInfo, string message)
        {
            ValidationType = validationType;
            _message = message;
            Stack = stack;
            _objectName = GetObjectsName(obj);
            _fieldName = memberInfo != null ? $"{memberInfo.DeclaringType}.{memberInfo.Name}" : null;
        }

        public ValidationError(Type validationType, string message, FieldInstanceData data)
        {
            ValidationType = validationType;
            _message = message;
            Stack = data != null ? data.Stack : new List<object>();
            _objectName = GetObjectsName(data?.Obj);
            _fieldName = data?.FieldInfo != null ? $"{data.FieldInfo.DeclaringType}.{data.FieldInfo.Name}" : null;
        }

        private static string GetObjectsName(object obj)
        {
            if (obj == null)
            {
                const string PROJECT_SCOPE_KEY = "Project";
                return PROJECT_SCOPE_KEY;
            }

            string nameWithHierarchy;
            if (obj is Component component)
            {
                var objectsHierarchy = new List<string>();
                var transform = component.transform;
                while (transform != null)
                {
                    objectsHierarchy.Insert(0, transform.name);
                    transform = transform.parent;
                }

                nameWithHierarchy = string.Join(".", objectsHierarchy);
            }
            else if (obj is UnityEngine.Object unityObject)
            {
                nameWithHierarchy = unityObject.name;
            }
            else
            {
                nameWithHierarchy = obj.ToString();
            }

            return nameWithHierarchy;
        }

        public override string ToString()
        {
            var obj = string.IsNullOrEmpty(_objectName) ? "" : $"{_objectName} -> ";
            var field = string.IsNullOrEmpty(_fieldName) ? "" : $"{_fieldName} -> ";
            return $"{obj}{field}{_message}";
        }
    }
}