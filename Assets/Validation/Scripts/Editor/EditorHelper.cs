namespace LazySloth.Validation
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;
    using Object = UnityEngine.Object;

    public static class EditorHelper
    {
        public static List<string> GetAssetsPathsByFilter(string filter, string path, List<string> excluded)
        {
            var assetGuids = AssetDatabase.FindAssets(filter);
            //Debug.Assert(assetGuids.Length > 0, $"Didn't find any assets under {path} path");

            var unfilteredAssetPaths = new string[assetGuids.Length];
            for (int i = 0; i < assetGuids.Length; i++)
            {
                unfilteredAssetPaths[i] = AssetDatabase.GUIDToAssetPath(assetGuids[i]);
            }

            //todo: use 'StartsWith' instead of 'Contains'?
            var assetPaths = unfilteredAssetPaths.Where(x => x.Contains(path)).ToList();

            foreach (var excludePath in excluded)
            {
                assetPaths.RemoveAll((s => s.StartsWith(excludePath)));
            }

            return assetPaths;
        }

        public static bool FieldIsVisibleInInspector(FieldInfo fieldInfo)
        {
            //anything else makes field non-serializable? Maybe lack of 'SerializableAttribute'?

            var isNotSerializedField = HasCustomAttribute<NonSerializedAttribute>(fieldInfo);
            var hasHideInInspectorAttribute = HasCustomAttribute<HideInInspector>(fieldInfo);
            //dictionaries are serialized with odin, it could be useful to add condition here that checks if odin is used
            var isDictionary = typeof(IDictionary).IsAssignableFrom(fieldInfo.FieldType);
            var isAction = typeof(MulticastDelegate).IsAssignableFrom(fieldInfo.FieldType);

            if (isNotSerializedField || hasHideInInspectorAttribute || isDictionary || isAction)
            {
                return false;
            }

            if (fieldInfo.IsPublic)
            {
                return true;
            }

            var isSerializeField = HasCustomAttribute<SerializeField>(fieldInfo) &&
                                   !HasCustomAttribute<HideInInspector>(fieldInfo);
            return isSerializeField;
        }

        public static bool HasCustomAttribute<T>(MemberInfo memberInfo) where T : Attribute
        {
            var attribute = GetCustomAttribute(memberInfo, typeof(T));
            var result = attribute != null;
            return result;
        }

        private static Attribute GetCustomAttribute(MemberInfo memberInfo, Type type)
        {
            var result = Attribute.GetCustomAttribute(memberInfo, type);
            return result;
        }

        public static bool IsUnityEngineObject(this object obj, out Object outCome)
        {
            if (!(obj is Object unityEngineObject))
            {
                outCome = null;
                return false;
            }

            outCome = unityEngineObject;
            return true;
        }

        public static bool UnityObjectIsNull(object obj)
        {
            if (obj is Object unityObject)
            {
                return unityObject == null;
            }
            
            if (obj is String s)
            {
                if (String.IsNullOrEmpty(s))
                {
                    return true;
                }
            }
            
            return obj == null;
        }
    }
}