namespace LazySloth.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;

    public class FullValidation
    {
        [MenuItem("Validation/Validate everything", false, -1)]
        public static void RunValidation()
        {
            var result = new ValidationResult("<b>Global validation</b>");

            var validationMethods = ValidationHelper.GetAllValidationMethods();

            PrintValidationNames(validationMethods);

            foreach (var validationMethod in validationMethods)
            {
                var instance = Activator.CreateInstance(validationMethod.DeclaringType);
                validationMethod.Invoke(instance, new object[] 
                { result, ValidationHelper.Config.ProjectMainFolderPath, ValidationHelper.Config.OutOfValidationPaths });
            }

            result.Print();
        }

        private static void PrintValidationNames(List<MethodInfo> validationMethods)
        {
            var validationClassNames = $"Validation method classes ({validationMethods.Count}):\n";
            foreach (var validationMethod in validationMethods)
            {
                validationClassNames += $"{validationMethod.DeclaringType}\n";
            }

            Debug.Log(validationClassNames);
        }
    }
}