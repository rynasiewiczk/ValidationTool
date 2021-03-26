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
            var instance = new ScenesValidation();
            instance.RunValidation(ValidationHelper.Config.ProjectMainFolderPath, ValidationHelper.Config.OutOfValidationPaths);
        }

        public override void RunValidation(string startPath, List<string> ignorePaths, ValidationResult result = null)
        {
            if (result == null)
            {
                result = new ValidationResult("<b>ScenesValidation</b>");
            }

            Validate(result, startPath, ignorePaths);
            result.Print();
        }

        [ValidateMethod]
        protected override void Validate(ValidationResult result, string startPath, List<string> ignorePaths)
        {
            if (IsAnyOpenSceneModified())
            {
                var popupAnswer = EditorUtility.DisplayDialogComplex("Scenes validation", "Do you want to save open scenes (not saved changes will be discarded)?", "Yes", "No", "Cancel");

                if (popupAnswer == 0) //clicked 'Yes'
                {
                    EditorSceneManager.SaveOpenScenes();
                }
                else if (popupAnswer == 1) //clicked 'No'
                { }
                else if (popupAnswer == 2) //clicked 'Cancel'
                {
                    return;
                }
            }

            var currentScenePath = SceneManager.GetActiveScene().path;
            var allOpenedScenePaths = GetLoadedScenesPaths();

            var scenePaths = EditorHelper.GetAssetsPathsByFilter(ValidationConfig.SCENES_FILTER_KEY,
                startPath,
                ignorePaths);

            scenePaths = scenePaths.Where(x => !ValidationHelper.Config.OutOfValidationSceneNames.Any(y => x.Contains(y))).ToList();

            var uniqueInstanceData = new List<FieldInstanceData>();

            foreach (var scenePath in scenePaths)
            {
                EditorUtility.DisplayProgressBar("Validate scenes", scenePath,
                    (float)scenePaths.IndexOf(scenePath) / scenePaths.Count);
                var scene = EditorSceneManager.OpenScene(scenePath);
                if (!scene.IsValid())
                {
                    Debug.Log($"Invalid scene: {scene.path}");
                    continue;
                }

                var fieldInstanceData = ValidationHelper.GetValidateableFields(scene);

                //Scenes validation needs to check for repetitions in every scene, because we cannot pass all scenes to get fields from them at once.
                //They have to be passed one by one. Because of that, we cannot filter repetitive fields inside the logic
                //that gets them -> it doesn't have all of them.
                var scenesUniqueInstanceData = new List<FieldInstanceData>();
                foreach (var instanceData in fieldInstanceData)
                {
                    if (scenesUniqueInstanceData.Any(x => x.Instance == instanceData.Instance && x.FieldInfo == instanceData.FieldInfo && x.Obj == instanceData.Obj))
                    {
                        continue;
                    }

                    scenesUniqueInstanceData.Add(instanceData);
                }

                var withError = new List<FieldInstanceData>();

                try
                {
                    foreach (var instanceData in scenesUniqueInstanceData)
                    {
                        if (uniqueInstanceData.Any(x => x.Instance == instanceData.Instance && x.FieldInfo == instanceData.FieldInfo && x.Obj == instanceData.Obj))
                    {
                        continue;
                    }

                        uniqueInstanceData.Add(instanceData);

                        if (EditorHelper.UnityObjectIsNull(instanceData.Instance))
                        {
                            var log =
                                $"Field is null.\n" +
                                $"Scene name: {scene.name}\n" +
                                $"GameObject name: {instanceData.Component.gameObject.name}\n" +
                                $"Component name: {instanceData.Component.GetType()}\n" +
                                $"Field type: {instanceData.FieldInfo.FieldType}\n" +
                                $"Field name: {instanceData.FieldInfo.Name}\n";

                            result.Add(GetType(), log, instanceData);

                            withError.Add(instanceData);
                        }
                    }
                }
                catch (Exception e)
                {
                    ValidationLogsHelper.AddValidationInternalErrorLog(typeof(ScenesValidation), result, e);
                }
            }

            EditorUtility.ClearProgressBar();

            EditorSceneManager.OpenScene(currentScenePath, OpenSceneMode.Single);
            foreach (var path in allOpenedScenePaths)
            {
                if(EditorSceneManager.GetSceneByPath(path).isLoaded)
                {
                    continue;
                }
                EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
            }
        }

        private bool IsAnyOpenSceneModified()
        {
            var scenes = GetLoadedScenes();
            return scenes.Any(x => x.isDirty);
        }

        private List<string> GetLoadedScenesPaths()
        {
            var scenes = GetLoadedScenes();
            return scenes.Select(x => x.path).ToList();
        }

        private List<Scene> GetLoadedScenes()
        {
            var scenes = new List<Scene>();
            var sceneCount = SceneManager.sceneCount;

            for (int i = 0; i < sceneCount; i++)
            {
                scenes.Add(SceneManager.GetSceneAt(i));
            }

            return scenes;
        }
    }
}