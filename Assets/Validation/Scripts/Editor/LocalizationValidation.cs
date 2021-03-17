namespace LazySloth.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Localization;
    using TMPro;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;

    public class LocalizationValidation : BaseValidation
    {
        private static readonly List<Type> TextComponentTypes = new List<Type>
            {typeof(Text), typeof(TextMeshProUGUI), typeof(TextMesh), typeof(TextMeshPro)};

        private static readonly List<Type> LocalizationComponentTypes = new List<Type>
            {typeof(LocalizedTextMarker), typeof(NotLocalizedTextMarker), typeof(ScriptLocalizedTextMarker)};


        [MenuItem("Validation/Localization")]
        public static void RunValidation()
        {
            RunValidation(ValidationHelper.Config.ProjectMainFolderPath, ValidationHelper.Config.OutOfValidationPaths);
        }

        public static void RunValidation(string startPath, List<string> ignorePaths)
        {
            var result = new ValidationResult("<b>LocalizationValidation</b>");
            var instance = new LocalizationValidation();
            instance.Validate(result, startPath, ignorePaths);

            result.Print();
        }

        //[ValidateMethod] ignore localization validation in full-validation process until there is proper localization
        protected override void Validate(ValidationResult result, string startPath, List<string > ignorePaths)
        {
            ValidateTextsInScenes(result, startPath, ignorePaths);
            ValidateTextsInPrefabs(result, startPath, ignorePaths);
        }

        private static void ValidateTextsInScenes(ValidationResult result, string startPath, List<string> ignorePaths)
        {
            var currentScenePath = SceneManager.GetActiveScene().path;
            var scenePaths = EditorHelper.GetAssetsPathsByFilter(ValidationConfig.SCENES_FILTER_KEY,
                startPath,
                ignorePaths);

            scenePaths = scenePaths.Where(x => !ValidationHelper.Config.OutOfValidationSceneNames.Any(y => x.Contains(y))).ToList();
            
            foreach (var scenePath in scenePaths)
            {
                EditorUtility.DisplayProgressBar("Validate Localization", scenePath,
                    (float) scenePaths.IndexOf(scenePath) / scenePaths.Count);
                var scene = EditorSceneManager.OpenScene(scenePath);
                if (!scene.IsValid())
                {
                    Debug.Log($"Invalid Localization: {scene.path}");
                    continue;
                }

                var textComponents = ValidationHelper.GetComponentsInActiveScene(TextComponentTypes, false);

                try
                {
                    foreach (var component in textComponents)
                    {
                        EditorUtility.DisplayProgressBar("Validate Localization", component.gameObject.name,
                            (float) textComponents.IndexOf(component) / textComponents.Count);

                        var allComponentsOnGameObject = component.gameObject.GetComponents<Component>();
                        var hasLocalizationComponent =
                            allComponentsOnGameObject.Any(x => LocalizationComponentTypes.Contains(x.GetType()));

                        if (!hasLocalizationComponent)
                        {
                            var rootParent = component.transform.root.gameObject;

                            var log =
                                $"GameObject has text component without localization component.\n" +
                                $"Scene name: {scene.name}\n" +
                                $"Source Game Object: {rootParent.name}\n" +
                                $"Game Object with component name: {component.gameObject.name}\n" +
                                $"Component type: {component.GetType()}\n";

                            result.Add(log, component.gameObject);
                        }
                    }
                }
                catch (Exception e)
                {
                    ValidationLogsHelper.AddValidationInternalErrorLog(typeof(LocalizationValidation), result, e);
                }
            }

            EditorUtility.ClearProgressBar();
            EditorSceneManager.OpenScene(currentScenePath);
        }

        private static void ValidateTextsInPrefabs(ValidationResult result, string startPath, List<string> ignorePaths)
        {
            var prefabPaths = EditorHelper.GetAssetsPathsByFilter(ValidationConfig.PREFABS_FILTER_KEY,
                startPath,
                ignorePaths);

            var abstractPaths = prefabPaths.Where(x => x.Contains("Abstract")).ToList();
            foreach (var abstractPath in abstractPaths)
            {
                prefabPaths.Remove(abstractPath);
            }

            var prefabs = new List<GameObject>();
            for (int i = 0; i < prefabPaths.Count; i++)
            {
                EditorUtility.DisplayProgressBar("Validate Localization", $"Loading {prefabPaths[i]}",
                    (float) i / prefabPaths.Count);
                prefabs.Add(AssetDatabase.LoadAssetAtPath<GameObject>(prefabPaths[i]));
            }

            EditorUtility.DisplayProgressBar("Validate Localization", "Getting localization fields", 0.99f);

            var textComponents = ValidationHelper.GetComponents(prefabs.ToArray(), TextComponentTypes, false);

            try
            {
                foreach (var component in textComponents)
                {
                    EditorUtility.DisplayProgressBar("Validate Localization", component.gameObject.name,
                        (float) textComponents.IndexOf(component) / textComponents.Count);

                    var allComponentsOnGameObject = component.gameObject.GetComponents<Component>();
                    var hasLocalizationComponent =
                        allComponentsOnGameObject.Any(x => LocalizationComponentTypes.Contains(x.GetType()));

                    if (!hasLocalizationComponent)
                    {
                        var rootParent = component.transform.root.gameObject;
                        var log =
                            $"GameObject has text component without localization component.\n" +
                            $"Source Prefab: {rootParent.name}\n" +
                            $"Game Object with component name: {component.gameObject.name}\n" +
                            $"Component type: {component.GetType()}\n";

                        result.Add(log, component.gameObject);
                    }
                }
            }
            catch (Exception e)
            {
                ValidationLogsHelper.AddValidationInternalErrorLog(typeof(LocalizationValidation), result, e);
            }

            EditorUtility.ClearProgressBar();
        }
    }
}