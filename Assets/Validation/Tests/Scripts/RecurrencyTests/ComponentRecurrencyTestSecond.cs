namespace LazySloth.Validation.Test
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "ValidationTestComponentSecond", menuName = "Validation/Test/ComponentSecond")]
    public class ComponentRecurrencyTestSecond : MonoBehaviour
    {
        [SerializeField] private ComponentRecurrencyTestFirst _referenceToFirst = default;
        [SerializeField] private ComponentRecurrencyTestSecond _referenceToSecond = default;
        [SerializeField] private ComponentRecurrencyTestThird _referenceToThird = default;
    }
}