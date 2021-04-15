namespace LazySloth.Validation.Test
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "ScriptableObjectReferenceTest", menuName = "Validation/Test/ScriptableObjectReferenceTest")]
    public class ScriptableObjectReferenceTest : ScriptableObject
    {
        [SerializeField] private ComponentRecurrencyTestFirst _providedPrefab = default;
        [SerializeField] private ComponentRecurrencyTestFirst _missingPrefab = default;

        [SerializeField] private ScriptableObjectReferenceTest _providedSo = default;
        [SerializeField] private ScriptableObjectReferenceTest _missingSo = default;

    }
}