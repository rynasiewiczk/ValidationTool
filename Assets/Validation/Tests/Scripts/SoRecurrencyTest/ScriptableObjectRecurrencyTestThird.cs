namespace LazySloth.Validation.Test
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "ValidationTestScriptableObjectThird", menuName = "Validation/Test/ScriptableObjectThird")]
    public class ScriptableObjectRecurrencyTestThird : ScriptableObject
    {
        [SerializeField] private ScriptableObjectRecurrencyTestFirst _referenceToFirst = default;
        [SerializeField] private ScriptableObjectRecurrencyTestSecond _referenceToSecond = default;
    }
}