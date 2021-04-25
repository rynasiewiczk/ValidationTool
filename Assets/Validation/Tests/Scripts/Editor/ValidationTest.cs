namespace LazySloth.Validation.Test
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    public class ValidationTest
    {
        private static ValidationTestConfig _config;

        public static ValidationTestConfig Config
        {
            get
            {
                if (_config == null)
                {
                    _config = Resources.Load<ValidationTestConfig>(ValidationTestConfig.CONFIG_RESOURCES_PATH);
                }

                return _config;
            }
        }

        [MenuItem("Tools/ValidationTool/Tests/Run tests %#&v")]
        public static ValidationResult RunTests()
        {
            var result = new ValidationResult("<b>Validation test run</b>");

            var validationMethods = ValidationHelper.GetAllValidationMethods();
            foreach (var validationMethod in validationMethods)
            {
                var instance = Activator.CreateInstance(validationMethod.DeclaringType);
                validationMethod.Invoke(instance, new object[] { result, Config.GetFolderPath(), new List<string>() });
            }

            result.Print();
            return result;
        }

        [MenuItem("Tools/ValidationTool/Tests/Run tests for scenes")]
        private static void RunTests_Scenes()
        {
            var instance = new ScenesValidation();
            instance.RunValidation(Config.GetFolderPath(), new List<string>());
        }

        [MenuItem("Tools/ValidationTool/Tests/Run tests for prefabs")]
        private static void RunTests_Prefabs()
        {
            var instance = new PrefabsValidation();
            instance.RunValidation(Config.GetFolderPath(), new List<string>());
        }

        [MenuItem("Tools/ValidationTool/Tests/Run tests for SOs")]
        private static void RunTests_ScriptableObjects()
        {
            var instance = new ScriptableObjectsValidation();
            instance.RunValidation(Config.GetFolderPath(), new List<string>());
        }
    }
}