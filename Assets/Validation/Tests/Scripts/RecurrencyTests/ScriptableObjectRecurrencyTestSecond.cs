namespace LazySloth.Validation.Test
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "ValidationTestScriptableObjectSecond", menuName = "Validation/Test/ScriptableObjectSecond")]
    public class ScriptableObjectRecurrencyTestSecond : ScriptableObject
    {
        [SerializeField] private ScriptableObjectRecurrencyTestFirst _referenceToFirst = default;
        [SerializeField] private ScriptableObjectRecurrencyTestSecond _referenceToSecond = default;
        [SerializeField] private ScriptableObjectRecurrencyTestThird _referenceToThird = default;
    }
}