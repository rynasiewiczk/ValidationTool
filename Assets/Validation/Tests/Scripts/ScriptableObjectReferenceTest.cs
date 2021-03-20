namespace LazySloth.Validation.Test
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "ScriptableObjectReferenceTest", menuName = "Validation/Test/ScriptableObjectReferenceTest")]
    public class ScriptableObjectReferenceTest : ScriptableObject
    {
        [SerializeField] private Component _providedComponent = default;
        [SerializeField] private Component _missingComponent = default;

        [SerializeField] private ComponentReferenceTest _providedPrefab = default;
        [SerializeField] private ComponentReferenceTest _missingPrefab = default;

        [SerializeField] private ScriptableObjectReferenceTest _providedSo = default;
        [SerializeField] private ScriptableObjectReferenceTest _missingSo = default;
    }
}