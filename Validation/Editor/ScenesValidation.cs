namespace LazySloth.Validation
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class ScenesValidation
    {
        [MenuItem("Validation/Scenes")] public static void RunValidation()
        {
            var result = new ValidationResult("<b>ScenesValidation</b>");
            Validate(result);

            result.Print();
        }

        [ValidateMethod] private static void Validate(ValidationResult result)
        {
            var currentScenePath = SceneManager.GetActiveScene().path;
            var scenePaths = EditorHelper.GetAssetsPathsByFilter(ValidationConfig.SCENES_FILTER_KEY, ValidationHelper.ValidationConfig.ProjectMainFolderPath);

            foreach (var scenePath in scenePaths)
            {
                var scene = EditorSceneManager.OpenScene(scenePath);
                if (!scene.IsValid())
                {
                    Debug.Log($"Invalid scene: {scene.path}");
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
                    if (uniqueInstanceData.Any(x => x.Instance == instanceData.Instance && x.FieldInfo == instanceData.FieldInfo))
                    {
                        continue;
                    }

                    uniqueInstanceData.Add(instanceData);
                }

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

            EditorSceneManager.OpenScene(currentScenePath);
        }
    }
}