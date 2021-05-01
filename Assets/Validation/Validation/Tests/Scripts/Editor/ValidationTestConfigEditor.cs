namespace LazySloth.Validation.Test
{
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(ValidationTestConfig))]
    public class ValidationTestConfigEditor : Editor
    {
        private static ValidationResult _lastRunValidationResults;
        private static GUIStyle _guiStyle;

        public override void OnInspectorGUI()
        {
            if(_guiStyle == null)
            {
                _guiStyle = GUI.skin.GetStyle("HelpBox");
                _guiStyle.fontSize = 12;
                _guiStyle.richText = true;
            }

            base.OnInspectorGUI();

            var instance = (ValidationTestConfig)target;

            GUILayout.Space(15);

            SetValidationButton();
            DisplayExpectationsForTestValidation();
            DrawValidationResults(instance);
        }

        private static void DisplayExpectationsForTestValidation()
        {
            GUILayout.Space(15);
            EditorGUILayout.HelpBox("Scenes validation should throw 34 errors in total. It includes components missing from the scenes, prefabs referenced in the scenes, and SOs referenced by the prefabs", MessageType.Info);

            GUILayout.Space(10);
            EditorGUILayout.HelpBox("Prefabs validation should throw 18 errors in total. It includes components missing from the prefabs and SOs referenced by them", MessageType.Info);

            GUILayout.Space(10);
            EditorGUILayout.HelpBox("SO validation should throw 6 errors in total. It includes components missing from the SOs and prefabs referenced from them", MessageType.Info);
        }

        private void SetValidationButton()
        {
            if (GUILayout.Button("Run test"))
            {
                _lastRunValidationResults = ValidationTest.RunTests();
            }
        }

        private void DrawValidationResults(ValidationTestConfig instance)
        {
            if (_lastRunValidationResults != null)
            {
                var sceneResults = _lastRunValidationResults.ValidationErrors.Where(x => x.ValidationType == typeof(ScenesValidation));
                var sceneExpectedErrorsCount = instance.SceneErrorsExpectedCount;

                var prefabResults = _lastRunValidationResults.ValidationErrors.Where(x => x.ValidationType == typeof(PrefabsValidation));
                var prefabExpectedErrorsCount = instance.PrefabErrorsExpectedCount;

                var soResults = _lastRunValidationResults.ValidationErrors.Where(x => x.ValidationType == typeof(ScriptableObjectsValidation));
                var soExpectedErrorsCount = instance.ScriptableObjectErrorsExpectedCount;

                var scenesValidationResultText =
                    $"Expected scene errors count: {sceneExpectedErrorsCount} " +
                    $"\nTest scene errors count: {sceneResults.Count()} " +
                    $"\n Scenes test passed: <b>{(sceneExpectedErrorsCount == sceneResults.Count() ? "<color=green>yes</color>" : "<color=red>no</color>")}</b>";

                var prefabsValidationResultText =
                    $"Expected prefab errors count: {prefabExpectedErrorsCount} " +
                    $"\nTest prefab errors count: {prefabResults.Count()} " +
                    $"\n Prefabs test passed: <b>{(prefabExpectedErrorsCount == prefabResults.Count() ? "<color=green>yes</color>" : "<color=red>no</color>")}</b>";

                var soValidationResultText =
                    $"Expected so errors count: {soExpectedErrorsCount} " +
                    $"\nTest so errors count: {soResults.Count()} " +
                    $"\n SOs test passed: <b>{(soExpectedErrorsCount == soResults.Count() ? "<color=green>yes</color>" : "<color=red>no</color>")}</b>";

                GUILayout.Space(15);

                

                EditorGUILayout.TextArea(scenesValidationResultText, _guiStyle);
                GUILayout.Space(5);
                
                EditorGUILayout.TextArea(prefabsValidationResultText, _guiStyle);
                GUILayout.Space(10);
                EditorGUILayout.TextArea(soValidationResultText, _guiStyle);
            }
        }
    }
}