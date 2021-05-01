namespace LazySloth.Validation.Test
{
    using UnityEngine;

    public class ComponentRecurrencyTestThird : MonoBehaviour
    {
        [SerializeField] private ComponentRecurrencyTestFirst _referenceToFirst = default;
        [SerializeField] private ComponentRecurrencyTestSecond _referenceToSecond = default;
        [SerializeField] private ComponentRecurrencyTestThird _referenceToThird = default;
    }
}