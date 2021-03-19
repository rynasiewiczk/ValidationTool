namespace LazySloth.Validation.Test
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "ValidationTestSoSecond", menuName = "Validation/Test/SoSecond")]
    public class SoRecurrencyTestSecond : ScriptableObject
    {
        [SerializeField] private SoRecurrencyTestFirst _referenceToFirst = default;
        [SerializeField] private SoRecurrencyTestThird _referenceToThird = default;
    }
}