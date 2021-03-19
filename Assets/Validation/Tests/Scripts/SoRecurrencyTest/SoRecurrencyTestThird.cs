namespace LazySloth.Validation.Test
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "ValidationTestSoThird", menuName = "Validation/Test/SoThird")]
    public class SoRecurrencyTestThird : ScriptableObject
    {
        [SerializeField] private SoRecurrencyTestFirst _referenceToFirst = default;
        [SerializeField] private SoRecurrencyTestSecond _referenceToSecond = default;
    }
}