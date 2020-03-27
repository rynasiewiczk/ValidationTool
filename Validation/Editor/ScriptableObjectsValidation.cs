namespace LazySloth.Validation
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    public class ScriptableObjectsValidation
    {
        [MenuItem("Validation/ScriptableObjects")] public static void RunValidation()
        {
            var result = new ValidationResult("<b>SO Validation</b>");
            Validate(result);
            result.Print();
        }

        [ValidateMethod]
        private static void Validate(ValidationResult result)
        {
            var paths = EditorHelper.GetAssetsPathsByFilter(ValidationConfig.SO_FILTER_KEY, ValidationHelper.ValidationConfig.ProjectMainFolderPath);
            var allSo = new List<ScriptableObject>();
            for (int i = 0; i < paths.Count; i++)
            {
                var asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(paths[i]);
                if (asset == null)
                {
                    result.Add($"Asset at path {paths[i]} is not a valid ScriptableObject");
                }
                else
                {
                    allSo.Add(asset);
                }
            }

            var fieldInstanceData = new List<FieldInstanceData>();
            fieldInstanceData.AddRange(ValidationHelper.GetValidateableFields(allSo));
            
            foreach (var instanceData in fieldInstanceData)
            {
                if (EditorHelper.UnityObjectIsNull(instanceData.Instance))
                {
                    var log =
                        $"Field is null.\n" +
                        $"SO name: {instanceData.FieldInfo.DeclaringType}\n" +
                        $"Field type: {instanceData.FieldInfo.FieldType}\n" +
                        $"Field name: {instanceData.FieldInfo.Name}\n";

                    result.Add(log, instanceData.Component, instanceData.FieldInfo);
                }
            }
        }
    }
}