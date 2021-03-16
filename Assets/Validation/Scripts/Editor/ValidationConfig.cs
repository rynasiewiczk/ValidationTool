namespace LazySloth.Validation
{
    using UnityEngine;
    using System.Collections.Generic;

    [CreateAssetMenu(fileName = "ValidationConfig", menuName = "Validation/Config")]
    public class ValidationConfig : ScriptableObject
    {
        public const string CONFIG_RESOURCES_PATH = "ValidationConfig";

        public const string SCENES_FILTER_KEY = "t:scene";
        public const string PREFABS_FILTER_KEY = "t:prefab";
        public const string SO_FILTER_KEY = "t:scriptableobject";

        public int MaxNumberOfValidationResults = 10;
        public string ProjectMainFolderPath = "Assets/";

        public List<string> OutOfValidationPaths = new List<string>();
        public List<string> OutOfValidationComponentTypes = new List<string>();

        public List<string> OutOfValidationNamespaces = new List<string>
        {
            "UnityEngine",
            "TMPro",
        };
        
        public List<string> OutOfValidationSceneNames = new List<string>
        {
        };
    }
}