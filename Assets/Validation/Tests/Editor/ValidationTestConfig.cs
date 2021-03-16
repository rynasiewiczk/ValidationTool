namespace LazySloth.Validation
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "ValidationTestConfig", menuName = "Validation/TestConfig")]
    public class ValidationTestConfig : ScriptableObject
    {
        [SerializeField] private int _sceneErrorsExpectedCount = 1;
        [SerializeField] private int _prefabErrorsExpectedCount = 1;
        [SerializeField] private int _scriptableObjectExpectedCount = 1;

        [SerializeField] private string _testFolderPath = "Assets/Validation/Tests";

        public static string CONFIG_RESOURCES_PATH = "ValidationTestConfig";

        public int SceneErrorsExpectedCount => _sceneErrorsExpectedCount;
        public int PrefabErrorsExpectedCount => _prefabErrorsExpectedCount;
        public int ScriptableObjectErrorsExpectedCount => _scriptableObjectExpectedCount;
        public string TestFolderPath => _testFolderPath;
    }
}