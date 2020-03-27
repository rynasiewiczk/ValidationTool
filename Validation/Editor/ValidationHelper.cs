namespace LazySloth.Validation
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class MemberInfoWithValue
    {
        public readonly object Object;
        public readonly MemberInfo MemberInfo;
        public readonly object Value;


        public MemberInfoWithValue(object @object, MemberInfo memberInfo, object value)
        {
            Object = @object;
            MemberInfo = memberInfo;
            Value = value;
        }
    }

    public static class ValidationHelper
    {
        private static ValidationConfig _validationConfig;

        public static ValidationConfig ValidationConfig
        {
            get
            {
                if (_validationConfig == null)
                {
                    _validationConfig = Resources.Load<ValidationConfig>(ValidationConfig.CONFIG_RESOURCES_PATH);
                }

                return _validationConfig;
            }
        }

        #region Exclusion definitions

        private static readonly List<Type> _outOfValidationComponentTypes = new List<Type>
        {
        };

        private static readonly List<string> _outOfValidationNamespaces = new List<string>
        {
            "UnityEngine",
            "TMPro",
            "Sirenix", //OdinInspector root namespace
        };

        #endregion

        public static List<FieldInstanceData> GetValidateableFields(List<ScriptableObject> sos)
        {
            var fieldInstanceData = new List<FieldInstanceData>();
            var visibleFields = new List<MemberInfoWithValue>();

            foreach (var so in sos)
            {
                if (so == null)
                {
                    continue;
                }
                if (!IsTypeValidateable(so.GetType()))
                {
                    continue;
                }

                visibleFields = GetFieldsVisibleInInspectorRecursive(so, visibleFields);
            }

            foreach (var fieldData in visibleFields)
            {
                fieldInstanceData.Add(new FieldInstanceData(fieldData.Object, fieldData.MemberInfo as FieldInfo, null, fieldData.Value));
            }

            return fieldInstanceData;
        }

        public static List<FieldInstanceData> GetValidateableFields(Scene scene)
        {
            var rootGameObjects = scene.GetRootGameObjects();
            var fieldInstanceData = GetValidateableFields(rootGameObjects);
            return fieldInstanceData;
        }

        public static List<FieldInstanceData> GetValidateableFields(GameObject[] gameObjects)
        {
            var fieldInstanceData = new List<FieldInstanceData>();
            var components = GetValidateableComponents(gameObjects);
            var processedComponents = new List<Component>();
            foreach (var component in components)
            {
                var visibleFields = new List<MemberInfoWithValue>();
                if (component == null)
                {
                    processedComponents.Add(component);
                    continue;
                }

                if (processedComponents.Contains(component))
                {
                    continue;
                }

                processedComponents.Add(component);

                visibleFields = GetFieldsVisibleInInspectorRecursive(component, visibleFields);
                foreach (var fieldData in visibleFields)
                {
                    fieldInstanceData.Add(new FieldInstanceData(fieldData.Object, fieldData.MemberInfo as FieldInfo, component, fieldData.Value));
                }
            }

            return fieldInstanceData;
        }

        private static List<Component> GetValidateableComponents(GameObject[] gameObjects)
        {
            var components = new List<Component>();
            foreach (var rootGameObject in gameObjects)
            {
                var newComponents = rootGameObject.GetComponentsInChildren<Component>(true);
                foreach (var c in newComponents)
                {
                    if (c == null)
                    {
                        Debug.LogWarning($"There is an invalid component somewhere under {rootGameObject.name} game object in it's hierarchy.");
                    }
                }

                components.AddRange(newComponents);
                components = components.Where(x => x != null && IsComponentValidateable(x)).ToList();
            }

            return components;
        }

        private static List<MemberInfoWithValue> GetFieldsVisibleInInspectorRecursive(object @object, List<MemberInfoWithValue> listOfFieldInfos)
        {
            if (@object == null || EditorHelper.UnityObjectIsNull(@object) || listOfFieldInfos.Any(x => x.Object == @object))
            {
                return listOfFieldInfos;
            }

            if (@object is Component component && !IsComponentValidateable(component))
            {
                return listOfFieldInfos;
            }

            var type = @object.GetType();
            var fieldInfos = MethodsHelper.GetNonStaticFieldInfos(type);

            foreach (var fieldInfo in fieldInfos)
            {
                if (!EditorHelper.FieldIsVisibleInInspector(fieldInfo) || !IsFieldValidateable(fieldInfo))
                {
                    continue;
                }

                var fieldValue = fieldInfo.GetValue(@object);

                if (listOfFieldInfos.Any(x => x.MemberInfo == fieldInfo && x.Value == fieldValue))
                {
                    continue;
                }

                if (!IsFieldValidateable(fieldInfo))
                {
                    continue;
                }

                listOfFieldInfos.Add(new MemberInfoWithValue(@object, fieldInfo, fieldValue));

                if (type.BaseType != typeof(object) && IsFieldValidateable(fieldInfo))
                {
                    if (fieldValue is IList list)
                    {
                        foreach (var element in list)
                        {
                            listOfFieldInfos.Add(new MemberInfoWithValue(@object, fieldInfo, element));
                            listOfFieldInfos = GetFieldsVisibleInInspectorRecursive(fieldValue, listOfFieldInfos);
                        }
                    }

                    listOfFieldInfos = GetFieldsVisibleInInspectorRecursive(fieldValue, listOfFieldInfos);
                }
            }

            return listOfFieldInfos;
        }

        #region Validateable checks

        public static bool IsTypeValidateable(Type type)
        {
            var result = !_outOfValidationComponentTypes.Contains(type);
            return result;
        }

        private static bool IsComponentValidateable(Component component)
        {
            if (component == null)
            {
                Debug.LogError("Found null component. Does a prefab has invalid script attached?");
                return false;
            }

            var nameSpace = component.GetType().Namespace;
            nameSpace = string.IsNullOrEmpty(nameSpace) ? "" : nameSpace.Split('.')[0];

            var validNamespace = string.IsNullOrEmpty(nameSpace) || !_outOfValidationNamespaces.Any(x => x.Contains(nameSpace));
            var validType = !_outOfValidationComponentTypes.Contains(component.GetType());
            return validNamespace && validType;
        }

        private static bool IsFieldValidateable(FieldInfo fieldInfo)
        {
            var validType = !_outOfValidationComponentTypes.Contains(fieldInfo.FieldType);
            var hasOptionalFieldAttribute = EditorHelper.HasCustomAttribute<OptionalObjectFieldAttribute>(fieldInfo);

            return validType && !hasOptionalFieldAttribute;
        }

        #endregion
    }
}