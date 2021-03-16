namespace LazySloth.Validation
{
    using System;
    using UnityEngine;

    public static class ValidationLogsHelper
    {
        public static void AddValidationInternalErrorLog(Type validationType, ValidationResult validationResult, Exception exception)
        {
            validationResult.Add("Validation internal error. Check logs for details");
            Debug.LogError($"An error occured when building {validationType.Name} log.\n\n Exception:\n {exception}");
        }
    }
}