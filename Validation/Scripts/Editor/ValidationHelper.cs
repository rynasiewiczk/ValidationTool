namespace LazySloth.Validation
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEditor.Callbacks;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class MemberInfoWithValue
    {
        public readonly object Object;
        public readonly MemberInfo MemberInfo;
        public readonly object Value;

        public readonly List<object> Stack = new List<object>();

        public MemberInfoWithValue(object @object, MemberInfo memberInfo, object value, List<object> stack)
        {
            Object = @object;
            MemberInfo = memberInfo;
            Value = value;
            Stack = stack;
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

        public static string AssetReferenceTypeName => "AssetReference";
        private static bool AssetReferenceTypeExists = false;

        [DidReloadScripts]
        private static void Reload()
        {
            AssetReferenceTypeExists = false;
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

                visibleFields = GetFieldsVisibleInInspectorRecursive(so, visibleFields, new List<object>());
                var newVisibleFields = visibleFields.Where(x => !currentVisibleFields.Contains(x)).ToList();
                foreach (var fieldData in newVisibleFields)
                {
                    if (fieldInstanceData.Any(x => x.Instance == fieldData.Value && x.FieldInfo == fieldData.MemberInfo && x.Obj == fieldData.Object))
                    {
                        continue;
                    }

                    object objectToUse;
                    if(fieldData.Object is Component component)
                    {
                        objectToUse = component;
                    }
                    else
                    {
                        objectToUse = so;
                    }

                    fieldInstanceData.Add(new FieldInstanceData(objectToUse, fieldData.MemberInfo as FieldInfo, null, fieldData.Value, fieldData.Stack));
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

                visibleFields = GetFieldsVisibleInInspectorRecursive(component, visibleFields, new List<object>());
                foreach (var fieldData in visibleFields)
                {
                    if(fieldInstanceData.Any(x => x.Instance == fieldData.Value && x.FieldInfo == fieldData.MemberInfo && x.Obj == fieldData.Object))
                    {
                        continue;
                    }

                    var componentToUse = fieldData.Object is Component objectsComponent ? objectsComponent : component;
                    fieldInstanceData.Add(new FieldInstanceData(componentToUse, fieldData));
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

        private static List<MemberInfoWithValue> GetFieldsVisibleInInspectorRecursive(object obj,
            List<MemberInfoWithValue> listOfFieldInfos, List<object> stack)
        {
            if (obj == null || EditorHelper.UnityObjectIsNull(obj) ||
                listOfFieldInfos.Any(x => x.Object == obj))
            {
                return listOfFieldInfos;
            }

            if (obj is Component component && !IsComponentValidateable(component))
            {
                return listOfFieldInfos;
            }

            if (obj is ScriptableObject so && !IsTypeValidateable(so.GetType()))
            {
                return listOfFieldInfos;
            }

            stack.Add(obj);

            var type = obj.GetType();
            var fieldInfos = MethodsHelper.GetNonStaticFieldInfos(type);

            foreach (var fieldInfo in fieldInfos)
            {
                if (!EditorHelper.FieldIsVisibleInInspector(fieldInfo) || !IsFieldValidateable(fieldInfo))
                {
                    continue;
                }

                var fieldValue = fieldInfo.GetValue(obj);

                if (listOfFieldInfos.Any(x => x.MemberInfo == fieldInfo && x.Value == fieldValue && x.Object == obj))
                {
                    continue;
                }


                listOfFieldInfos.Add(new MemberInfoWithValue(obj, fieldInfo, fieldValue, new List<object>(stack)));

                if (type.BaseType != typeof(object) && IsFieldValidateable(fieldInfo))
                {
                    if (fieldValue is IList list)
                    {
                        foreach (var element in list)
                        {
                            listOfFieldInfos.Add(new MemberInfoWithValue(obj, fieldInfo, element, new List<object>(stack)));
                            listOfFieldInfos = GetFieldsVisibleInInspectorRecursive(fieldValue, listOfFieldInfos, stack);
                        }
                    }

                    if (!IsObjectAssetReferenceType(fieldValue))
                    {
                        listOfFieldInfos = GetFieldsVisibleInInspectorRecursive(fieldValue, listOfFieldInfos, stack); // in AddressableReference there are fields empty by default, and we don't want to include them
                    }
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

        public static bool IsObjectAssetReferenceType(object obj)
        {
            if (AssetReferenceTypeExists)
            {
                return IsObjectOfTheType();
            }

            var t = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                     from type in assembly.GetTypes()
                     where type.Name == AssetReferenceTypeName
                     select type).FirstOrDefault();

            if (t != null)
            {
                AssetReferenceTypeExists = true;
                return IsObjectOfTheType();
            }

            bool IsObjectOfTheType()
            {
                var objType = obj?.GetType();
                return obj?.GetType().Name == AssetReferenceTypeName;
            }

            return false;
        }

        #region Validateable checks

        public static bool IsTypeValidateable(Type type)
        {
            var validNamespace = IsNamespaceValidateable(type);
            var result = validNamespace && !Config.OutOfValidationComponentTypeNames.Contains(type.FullName);
            
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
            var validType = !Config.OutOfValidationComponentTypeNames.Contains(component.name);
            return validNamespace && validType;
        }

        private static bool IsFieldValidateable(FieldInfo fieldInfo)
        {
            var validType = !Config.OutOfValidationComponentTypeNames.Contains(fieldInfo.FieldType.Name);
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