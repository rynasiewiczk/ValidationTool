namespace LazySloth.Validation.Test
{
    using UnityEngine;

    public class ValidationTestGameObject : MonoBehaviour
    {
        [SerializeField] private GameObject _providedGameObject = default;
        [SerializeField] private GameObject _missingGameObject = default;
    }
}