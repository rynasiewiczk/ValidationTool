namespace LazySloth.Validation
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEngine;

    public class ValidationResult
    {
        public int MaxNumberOfLogs => 10;

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
                var count = Mathf.Min(logs.Count, result.MaxNumberOfLogs);
                for (int i = 0; i < count; i++)
                {
                    Debug.Log(logs[i]);
                }

                if (logs.Count >= result.MaxNumberOfLogs)
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