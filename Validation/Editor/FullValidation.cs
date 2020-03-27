namespace LazySloth.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;

    public class FullValidation
    {
        [MenuItem("Validation/Validate everything", false, -1)]
        public static void RunValidation()
        {
            var result = new ValidationResult("<b>Global validation</b>");

            var assembly = Assembly.GetCallingAssembly();

            var types = assembly.GetTypes();
            var validationMethods = new List<MethodInfo>();
            foreach (var type in types)
            {
                validationMethods.AddRange(type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                    .Where(x => x.GetCustomAttribute<ValidateMethodAttribute>() != null));
            }

            PrintValidationNames(validationMethods);

            foreach (var validationMethod in validationMethods)
            {
                var instance = Activator.CreateInstance(validationMethod.DeclaringType);
                validationMethod.Invoke(instance, new[] {result});
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