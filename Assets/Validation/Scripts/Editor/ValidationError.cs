namespace LazySloth.Validation
{
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEngine;

    public class ValidationError
    {
        private readonly string _objectName;
        private readonly string _fieldName;
        private readonly string _message;

        public ValidationError(object obj, MemberInfo fieldName, string message)
        {
            _objectName = GetObjectsName(obj);
            _fieldName = fieldName != null ? $"{fieldName.DeclaringType}.{fieldName.Name}" : null;
            _message = message;
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
            else if (obj is Object unityObject)
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
            var obj = string.IsNullOrEmpty(_objectName) ? "" : $"{_objectName} | ";
            var field = string.IsNullOrEmpty(_fieldName) ? "" : $"{_fieldName} | ";
            return $"{obj}{field}{_message}";
        }
    }
}