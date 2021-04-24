namespace LazySloth.Validation
{
    using System.IO;
    using UnityEditor;

    [CustomEditor(typeof(ValidationConfig))]
    public class ValidationConfigEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var t = (ValidationConfig)target;
            var mainFolderPath = t.ProjectMainFolderPath;
            if (!Directory.Exists(mainFolderPath))
            {
                EditorGUILayout.HelpBox($"Value set for {nameof(t.ProjectMainFolderPath)} is not a valid project path.", MessageType.Error);
            }
        }
    }
}