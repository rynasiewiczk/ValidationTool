namespace LazySloth.Validation.Test
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "ValidationTestScriptableObjectFirst", menuName = "Validation/Test/ScriptableObjectFirst")]
    public class ScriptableObjectRecurrencyTestFirst : ScriptableObject
    {
        [SerializeField] private ScriptableObjectRecurrencyTestFirst _referenceToFirst = default;
        [SerializeField] private ScriptableObjectRecurrencyTestSecond _referenceToSecond = default;
        [SerializeField] private ScriptableObjectRecurrencyTestThird _referenceToThird = default;
    }
}