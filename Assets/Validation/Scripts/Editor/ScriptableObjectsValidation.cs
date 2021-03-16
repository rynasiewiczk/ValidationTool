namespace LazySloth.Validation
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    public class ScriptableObjectsValidation : BaseValidation
    {
        [MenuItem("Validation/ScriptableObjects")]
        public static void RunValidation()
        {
            var result = new ValidationResult("<b>SO Validation</b>");
            var instance = new ScriptableObjectsValidation();
            instance.Validate(result, ValidationHelper.Config.ProjectMainFolderPath);
            
            result.Print();
        }

        [ValidateMethod]
        protected override void Validate(ValidationResult result, string startPath)
        {
            var paths = EditorHelper.GetAssetsPathsByFilter(ValidationConfig.SO_FILTER_KEY,
                startPath,
                ValidationHelper.Config.OutOfValidationPaths);
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

            try
            {
                foreach (var instanceData in fieldInstanceData)
                {
                    EditorUtility.DisplayProgressBar("Validate ScriptableObjects", instanceData.ToString(),
                        (float) fieldInstanceData.IndexOf(instanceData) / fieldInstanceData.Count);

                    if (EditorHelper.UnityObjectIsNull(instanceData.Instance))
                    {
                        var soName = instanceData?.Object != null ? instanceData.Object.ToString() : "";
                        var soNameLog = string.IsNullOrEmpty(soName)
                            ? ""
                            : $"ScriptableObject name: {soName}\n";

                        var log =
                            $"Field is null.\n" +
                            soNameLog +
                            $"SO type: {instanceData.FieldInfo.DeclaringType}\n" +
                            $"Field type: {instanceData.FieldInfo.FieldType}\n" +
                            $"Field name: {instanceData.FieldInfo.Name}\n";

                        result.Add(log, instanceData.Component, instanceData.FieldInfo);
                    }
                }
            }
            catch (Exception e)
            {
                ValidationLogsHelper.AddValidationInternalErrorLog(typeof(ScriptableObjectsValidation), result, e);
            }

            EditorUtility.ClearProgressBar();
        }
    }
}