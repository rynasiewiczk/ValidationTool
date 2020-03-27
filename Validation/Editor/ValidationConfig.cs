namespace LazySloth.Validation
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "ValidationConfig", menuName = "Validation/Config")]
    public class ValidationConfig : ScriptableObject
    {
        public const string CONFIG_RESOURCES_PATH = "Validation/ValidationConfig";
        
        public const string SCENES_FILTER_KEY = "t:scene";
        public const string PREFABS_FILTER_KEY = "t:prefab";
        public const string SO_FILTER_KEY = "t:scriptableobject";
        
        public string ProjectMainFolderPath = "Assets/_Project/";
    }
}