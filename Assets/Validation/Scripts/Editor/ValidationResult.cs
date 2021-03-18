namespace LazySloth.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEngine;

    public class ValidationResult
    {
        private readonly string _validationMethodName;
        public readonly List<ValidationError> ValidationErrors = new List<ValidationError>();

        public int ErrorsCount => ValidationErrors.Count;
        public bool Success => ErrorsCount == 0;

        public ValidationResult(string name)
        {
            _validationMethodName = name;
        }

        public void Add(Type validationType, string message, object obj = null, MemberInfo memberInfo = null)
        {
            var error = new ValidationError(validationType, obj, memberInfo, message);
            ValidationErrors.Add(error);
        }

        public override string ToString()
        {
            var errorsCount = ValidationErrors.Count;
            var result = Success
                ? $"<color=green>{_validationMethodName}: no errors</color>"
                : $"<color=red>{_validationMethodName}: {errorsCount} errors.</color>";
            return result;
        }

        public List<string> GetLogs()
        {
            return ValidationErrors.Select(validationError => validationError.ToString()).ToList();
        }
    }

    public static class ValidationResultHelper
    {
        public static void Print(this ValidationResult result)
        {
            if (!result.Success)
            {
                Debug.LogError(result.ToString());
                var logs = result.GetLogs();
                var maxNumberOfLogs = ValidationHelper.Config.MaxNumberOfValidationResults;
                var count = Mathf.Min(logs.Count, maxNumberOfLogs);
                var log = string.Empty;
                for (int i = 0; i < count; i++)
                {
                    log += logs[i] + "\n";
                }
                
                Debug.Log(log);

                if (logs.Count >= maxNumberOfLogs)
                {
                    Debug.LogError("More validation error logs truncated...");
                }
            }
            else
            {
                Debug.Log("<b>Validation succeeded!</b>");
            }
        }
    }
}