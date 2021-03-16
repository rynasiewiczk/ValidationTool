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
        private static ValidationConfig _config;

        public static ValidationConfig Config
        {
            get
            {
                if (_config == null)
                {
                    _config = Resources.Load<ValidationConfig>(ValidationConfig.CONFIG_RESOURCES_PATH);
                }

                return _config;
            }
        }

        public static List<FieldInstanceData> GetValidateableFields(List<ScriptableObject> sos)
        {
            var fieldInstanceData = new List<FieldInstanceData>();
            var visibleFields = new List<MemberInfoWithValue>();

            var currentVisibleFields = new List<MemberInfoWithValue>();
            
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
                var fields = currentVisibleFields;
                var newVisibleFields = visibleFields.Where(x => !fields.Contains(x));
                foreach (var fieldData in newVisibleFields)
                {
                    fieldInstanceData.Add(new FieldInstanceData(so, fieldData.MemberInfo as FieldInfo, null,
                        fieldData.Value));
                }

                currentVisibleFields = new List<MemberInfoWithValue>(visibleFields);
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
            var components = GetComponents(gameObjects, true);
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
                    var componentToUse = fieldData.Object is Component objectsComponent ? objectsComponent : component;
                    fieldInstanceData.Add(new FieldInstanceData(fieldData.Object, fieldData.MemberInfo as FieldInfo,
                        componentToUse, fieldData.Value));
                }
            }

            return fieldInstanceData;
        }

        public static List<Component> GetComponents(GameObject[] gameObjects, List<Type> componentTypes, bool validateable)
        {
            var allComponents = GetComponents(gameObjects, validateable);
            var componentOfGivenTypes = allComponents.Where(x => componentTypes.Contains(x.GetType())).ToList();
            return componentOfGivenTypes;
        }
        
        public static List<Component> GetComponentsInActiveScene(List<Type> componentTypes, bool validateable)
        {
            var gameObjects = GameObject.FindObjectsOfType<GameObject>();
            var allComponents = GetComponents(gameObjects, validateable);
            var componentOfGivenTypes = allComponents.Where(x => componentTypes.Contains(x.GetType())).ToList();
            return componentOfGivenTypes;
        }
        
        private static List<Component> GetComponents(GameObject[] gameObjects, bool validateable)
        {
            var components = new List<Component>();
            foreach (var gameObject in gameObjects)
            {
                var newComponents = gameObject.GetComponentsInChildren<Component>(true).ToList();
                foreach (var c in newComponents)
                {
                    if (c == null)
                    {
                        Debug.LogWarning(
                            $"There is an invalid component somewhere under {gameObject.name} game object in it's hierarchy.");
                    }
                }

                for (int i = newComponents.Count - 1; i >= 0; i--)
                {
                    if (components.Contains(newComponents[i]))
                    {
                        newComponents.Remove(newComponents[i]);
                    }
                }

                components.AddRange(newComponents);
                components = components.Where(x => x != null && (!validateable || IsComponentValidateable(x))).ToList();
            }

            return components;
        }

        private static List<MemberInfoWithValue> GetFieldsVisibleInInspectorRecursive(object @object,
            List<MemberInfoWithValue> listOfFieldInfos)
        {
            if (@object == null || EditorHelper.UnityObjectIsNull(@object) ||
                listOfFieldInfos.Any(x => x.Object == @object))
            {
                return listOfFieldInfos;
            }

            if (@object is Component component && !IsComponentValidateable(component))
            {
                return listOfFieldInfos;
            }

            if (@object is ScriptableObject so && !IsTypeValidateable(so.GetType()))
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

                //if (!IsFieldValidateable(fieldInfo))
                //{
                //    continue;
                //}

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

        public static List<MethodInfo> GetAllValidationMethods()
        {
            var assembly = Assembly.GetCallingAssembly();

            var types = assembly.GetTypes();
            var validationMethods = new List<MethodInfo>();
            foreach (var type in types)
            {
                validationMethods.AddRange(type
                    .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(x => x.GetCustomAttribute<ValidateMethodAttribute>() != null));
            }

            return validationMethods;
        }

        #region Validateable checks

        public static bool IsTypeValidateable(Type type)
        {
            var validNamespace = IsNamespaceValidateable(type);
            var result = validNamespace && !Config.OutOfValidationComponentTypes.Contains(type.FullName);
            
            return result;
        }

        private static bool IsComponentValidateable(Component component)
        {
            if (component == null)
            {
                Debug.LogError("Found null component. Does a prefab has invalid script attached?");
                return false;
            }

            var validNamespace = IsNamespaceValidateable(component.GetType());
            var validType = !Config.OutOfValidationComponentTypes.Contains(component.name);
            return validNamespace && validType;
        }

        private static bool IsFieldValidateable(FieldInfo fieldInfo)
        {
            var validType = !Config.OutOfValidationComponentTypes.Contains(fieldInfo.FieldType.Name);
            var hasOptionalFieldAttribute = EditorHelper.HasCustomAttribute<OptionalObjectFieldAttribute>(fieldInfo);

            return validType && !hasOptionalFieldAttribute;
        }

        private static bool IsNamespaceValidateable(Type type)
        {
            var nameSpace = type.Namespace;
            nameSpace = string.IsNullOrEmpty(nameSpace) ? "" : nameSpace.Split('.')[0];
        
            var validNamespace = string.IsNullOrEmpty(nameSpace) ||
                                 !Config.OutOfValidationNamespaces.Any(x => x.Contains(nameSpace));
            return validNamespace;
        }

        #endregion
    }
}