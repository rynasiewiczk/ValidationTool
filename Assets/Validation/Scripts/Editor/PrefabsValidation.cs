namespace LazySloth.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    public class PrefabsValidation : BaseValidation
    {
        [MenuItem("Validation/Prefabs")]
        public static void RunValidation()
        {
            var instance = new PrefabsValidation();
            instance.RunValidation(ValidationHelper.Config.ProjectMainFolderPath, ValidationHelper.Config.OutOfValidationPaths);
        }

        public override void RunValidation(string startPath, List<string> ignorePaths, ValidationResult result = null)
        {
            if (result == null)
            {
                result = new ValidationResult("<b>PrefabsValidation</b>");
            }
            
            Validate(result, startPath, ignorePaths);
            result.Print();
        }

        [ValidateMethod]
        protected override void Validate(ValidationResult result, string startPath, List<string> ignorePaths)
        {
            var prefabPaths = EditorHelper.GetAssetsPathsByFilter(ValidationConfig.PREFABS_FILTER_KEY,
                startPath,
                ignorePaths);

            //todo: chanfe 'Contains' to 'EndsWith'?
            var abstractPaths = prefabPaths.Where(x => x.Contains("Abstract")).ToList();
            foreach (var abstractPath in abstractPaths)
            {
                prefabPaths.Remove(abstractPath);
            }

            var prefabs = new List<GameObject>();
            for (int i = 0; i < prefabPaths.Count; i++)
            {
                EditorUtility.DisplayProgressBar("Validate Prefabs", $"Loading {prefabPaths[i]}",
                    (float) i / prefabPaths.Count);
                prefabs.Add(AssetDatabase.LoadAssetAtPath<GameObject>(prefabPaths[i]));
            }

            EditorUtility.DisplayProgressBar("Validate Prefabs", "Getting Prefabs fields", 0.99f);

            var fieldInstanceData = ValidationHelper.GetValidateableFields(prefabs.ToArray());

            try
            {
                foreach (var instanceData in fieldInstanceData)
                {
                    EditorUtility.DisplayProgressBar("Validate Prefabs", instanceData.Obj.ToString(),
                        (float) fieldInstanceData.IndexOf(instanceData) / fieldInstanceData.Count);
                    if (EditorHelper.UnityObjectIsNull(instanceData.Instance))
                    {
                        var log =
                            $"Field is null.\n" +
                            $"Source prefab name: {instanceData.Component.name}\n" +
                            $"Component name: {instanceData.Component.GetType()}\n" +
                            $"Field type: {instanceData.FieldInfo.FieldType}\n" +
                            $"Field name: {instanceData.FieldInfo.Name}\n" +
                            $"(Game)Object name: {(instanceData.Obj is GameObject go ? go.name : instanceData.Obj)}\n";

                        result.Add(log, instanceData.Component, instanceData.FieldInfo);
                    }
                }
            }
            catch (Exception e)
            {
                ValidationLogsHelper.AddValidationInternalErrorLog(typeof(PrefabsValidation), result, e);
            }

            EditorUtility.ClearProgressBar();
        }
    }
}