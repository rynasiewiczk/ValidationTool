namespace LazySloth.Validation
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEngine;

    public class ValidationResult
    {
        private readonly string _validationMethodName;
        private readonly List<ValidationError> _validationErrors = new List<ValidationError>();

        public ValidationResult(string name)
        {
            _validationMethodName = name;
        }

        public bool Success => _validationErrors.Count == 0;

        public void Add(string message, object obj = null, MemberInfo memberInfo = null)
        {
            var error = new ValidationError(obj, memberInfo, message);
            _validationErrors.Add(error);
        }

        public override string ToString()
        {
            var errorsCount = _validationErrors.Count;
            var result = Success
                ? $"<color=green>{_validationMethodName}: no errors</color>"
                : $"<color=red>{_validationMethodName}: {errorsCount.ToString()} errors.</color>";
            return result;
        }

        public List<string> GetLogs()
        {
            return _validationErrors.Select(validationError => validationError.ToString()).ToList();
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