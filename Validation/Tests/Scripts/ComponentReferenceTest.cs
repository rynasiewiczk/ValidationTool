namespace LazySloth.Validation.Test
{
    using UnityEngine;

    public class ComponentReferenceTest : MonoBehaviour
    {
        [SerializeField] private GameObject _providedGameObject = default;
        [SerializeField] private GameObject _missingGameObject = default;

        [SerializeField] private Component _providedComponent = default;
        [SerializeField] private Component _missingComponent = default;

        [SerializeField] private ComponentReferenceTest _providedPrefab = default;
        [SerializeField] private ComponentReferenceTest _missingPrefab = default;

        [SerializeField] private ScriptableObjectReferenceTest _providedSo = default;
        [SerializeField] private ScriptableObjectReferenceTest _missingSo = default;
    }
}