namespace LazySloth.Validation.Test
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "ValidationTestSoFirst", menuName = "Validation/Test/SoFirst")]
    public class SoRecurrencyTestFirst : ScriptableObject
    {
        [SerializeField] private SoRecurrencyTestSecond _referenceToSecond = default;
        [SerializeField] private SoRecurrencyTestThird _referenceToThird = default;
    }
}