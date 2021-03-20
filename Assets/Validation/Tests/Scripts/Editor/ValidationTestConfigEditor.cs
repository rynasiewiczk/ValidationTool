namespace LazySloth.Validation.Test
{
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(ValidationTestConfig))]
    public class ValidationTestConfigEditor : Editor
    {
        private static ValidationResult _lastRunValidationResults;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var instance = (ValidationTestConfig)target;

            GUILayout.Space(15);

            SetValidationButton();
            DrawValidationResults(instance);
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

                GUIStyle myStyle = GUI.skin.GetStyle("HelpBox");
                myStyle.fontSize = 13;
                myStyle.richText = true;

                EditorGUILayout.TextArea(scenesValidationResultText, myStyle);
                GUILayout.Space(10);
                EditorGUILayout.TextArea(prefabsValidationResultText, myStyle);
                GUILayout.Space(10);
                EditorGUILayout.TextArea(soValidationResultText, myStyle);
            }
        }
    }
}