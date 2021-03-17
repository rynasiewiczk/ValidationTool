namespace LazySloth.Validation
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

        [MenuItem("Validation/Tests/Run tests %#&v")]
        private static void RunTests()
        {
            var result = new ValidationResult("<b>Validation test run</b>");

            var validationMethods = ValidationHelper.GetAllValidationMethods();
            foreach (var validationMethod in validationMethods)
            {
                var instance = Activator.CreateInstance(validationMethod.DeclaringType);
                validationMethod.Invoke(instance, new object[] { result, Config.TestFolderPath, new List<string>() });
            }

            result.Print();
        }

        [MenuItem("Validation/Tests/Run tests for scenes")]
        private static void RunTests_Scenes()
        {
            ScenesValidation.RunValidation(Config.TestFolderPath, new List<string>());
        }

        [MenuItem("Validation/Tests/Run tests for prefabs")]
        private static void RunTests_Prefabs()
        {
            PrefabsValidation.RunValidation();
        }

        [MenuItem("Validation/Tests/Run tests for SOs")]
        private static void RunTests_ScriptableObjects()
        {
            ScriptableObjectsValidation.RunValidation();
        }
    }
}