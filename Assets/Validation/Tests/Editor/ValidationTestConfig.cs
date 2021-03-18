namespace LazySloth.Validation.Test
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "ValidationTestConfig", menuName = "Validation/TestConfig")]
    public class ValidationTestConfig : ScriptableObject
    {
        private const string _resourcesFolderNameAndSlash = "Resources/";
        private const string _scriptableObjectFilenameExtension = ".asset";

        [SerializeField] private int _sceneErrorsExpectedCount = 1;
        [SerializeField] private int _prefabErrorsExpectedCount = 1;
        [SerializeField] private int _scriptableObjectExpectedCount = 1;

        public static string CONFIG_RESOURCES_PATH = "ValidationTestConfig";

        public int SceneErrorsExpectedCount => _sceneErrorsExpectedCount;
        public int PrefabErrorsExpectedCount => _prefabErrorsExpectedCount;
        public int ScriptableObjectErrorsExpectedCount => _scriptableObjectExpectedCount;
        public int TestErrorsExpectedCount => SceneErrorsExpectedCount + PrefabErrorsExpectedCount + ScriptableObjectErrorsExpectedCount;

        /// <summary>
        /// Get path to main test folder that contains the config SO
        /// </summary>
        /// <param name="baseValidation"></param>
        /// <returns></returns>
        public string GetFolderPath()
        {
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:ValidationTestConfig");
            if (guids.Length == 0)
            {
                Debug.LogError($"{typeof(ValidationTestConfig)} instance not found.");
                return null;
            }
            else if (guids.Length > 1)
            {
                Debug.LogWarning($"Found more than one instance of {typeof(ValidationTestConfig)}. Using first one.");
            }

            string pathToConfig = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
            var configAsset = UnityEditor.AssetDatabase.LoadAssetAtPath(pathToConfig, typeof(ValidationTestConfig));

            var name = configAsset.name;
            var configSubstringToRemove = _resourcesFolderNameAndSlash + name + _scriptableObjectFilenameExtension;
            var pathToTestFolder = pathToConfig.Replace(configSubstringToRemove, null);

            return pathToTestFolder;
        }
    }
}