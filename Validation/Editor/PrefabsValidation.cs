namespace LazySloth.Validation
{
    using UnityEditor;
    using UnityEngine;

    public class PrefabsValidation
    {
        [MenuItem("Validation/Prefabs")] public static void RunValidation()
        {
            var result = new ValidationResult("<b>PrefabsValidation</b>");
            Validate(result);

            result.Print();
        }

        [ValidateMethod] private static void Validate(ValidationResult result)
        {
            var prefabPaths = EditorHelper.GetAssetsPathsByFilter(ValidationConfig.PREFABS_FILTER_KEY, ValidationHelper.ValidationConfig.ProjectMainFolderPath);
            var prefabs = new GameObject[prefabPaths.Count];
            for (int i = 0; i < prefabPaths.Count; i++)
            {
                prefabs[i] = (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPaths[i]));
            }

            var fieldInstanceData = ValidationHelper.GetValidateableFields(prefabs);
            foreach (var instanceData in fieldInstanceData)
            {
                if (EditorHelper.UnityObjectIsNull(instanceData.Instance))
                {
                    var log =
                        $"Field is null.\n" +
                        $"Source prefab name: {instanceData.Component.name}\n" +
                        $"Component name: {instanceData.Component.GetType()}\n" +
                        $"Field type: {instanceData.FieldInfo.FieldType}\n" +
                        $"Field name: {instanceData.FieldInfo.Name}\n" +
                        $"(Game)Object name: {(instanceData.Object is GameObject go ? go.name : instanceData.Object)}\n";

                    result.Add(log, instanceData.Component, instanceData.FieldInfo);
                }
            }
        }
    }
}