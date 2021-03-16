namespace LazySloth.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class ScenesValidation : BaseValidation
    {
        [MenuItem("Validation/Scenes")]
        public static void RunValidation()
        {
            var result = new ValidationResult("<b>ScenesValidation</b>");
            var instance = new ScenesValidation();
            instance.Validate(result, ValidationHelper.Config.ProjectMainFolderPath);

            result.Print();
        }

        [ValidateMethod]
        protected override void Validate(ValidationResult result, string startPath)
        {
            var currentScenePath = SceneManager.GetActiveScene().path;
            var scenePaths = EditorHelper.GetAssetsPathsByFilter(ValidationConfig.SCENES_FILTER_KEY,
                startPath,
                ValidationHelper.Config.OutOfValidationPaths);

            scenePaths = scenePaths.Where(x => !ValidationHelper.Config.OutOfValidationSceneNames.Any(y => x.Contains(y))).ToList();
            
            foreach (var scenePath in scenePaths)
            {
                EditorUtility.DisplayProgressBar("Validate scenes", scenePath,
                    (float) scenePaths.IndexOf(scenePath) / scenePaths.Count);
                var scene = EditorSceneManager.OpenScene(scenePath);
                if (!scene.IsValid())
                {
                    Debug.Log($"Invalid Scenes: {scene.path}");
                    continue;
                }

                var fieldInstanceData = ValidationHelper.GetValidateableFields(scene);

                //Scenes validation needs this explicitly, because we cannot pass all scenes to get fields from them at once.
                //They have to be passed one by one. Because of that, we cannot filter repetitive fields inside the logic
                //that gets them -> it doesn't have all of them.
                var uniqueInstanceData =
                    new List<FieldInstanceData>();
                foreach (var instanceData in fieldInstanceData)
                {
                    if (uniqueInstanceData.Any(x =>
                        x.Instance == instanceData.Instance && x.FieldInfo == instanceData.FieldInfo))
                    {
                        continue;
                    }

                    uniqueInstanceData.Add(instanceData);
                }

                try
                {
                    foreach (var instanceData in uniqueInstanceData)
                    {
                        if (EditorHelper.UnityObjectIsNull(instanceData.Instance))
                        {
                            var log =
                                $"Field is null.\n" +
                                $"Scene name: {scene.name}\n" +
                                $"GameObject name: {instanceData.Component.gameObject.name}\n" +
                                $"Component name: {instanceData.Component.GetType()}\n" +
                                $"Field type: {instanceData.FieldInfo.FieldType}\n" +
                                $"Field name: {instanceData.FieldInfo.Name}\n";

                            result.Add(log, instanceData.Component, instanceData.FieldInfo);
                        }
                    }
                }
                catch (Exception e)
                {
                    ValidationLogsHelper.AddValidationInternalErrorLog(typeof(ScenesValidation), result, e);
                }
            }

            EditorUtility.ClearProgressBar();
            EditorSceneManager.OpenScene(currentScenePath);
        }
    }
}