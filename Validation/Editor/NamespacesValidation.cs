namespace LazySloth.Validation
{
    using System.Linq;
    using System.Reflection;
    using UnityEditor;

    public class NamespacesValidation
    {
        [MenuItem("Validation/Namespaces")]
        public static void RunValidation()
        {
            var result = new ValidationResult("Namespaces validation");
            Validate(result);
            
            result.Print();
        }

        [ValidateMethod]
        private static void Validate(ValidationResult result)
        {
            var assembly = Assembly.Load("Assembly-CSharp");
            var classes = assembly.GetTypes().Where(x => x.IsClass);

            foreach (var c in classes)
            {
                if(!ValidationHelper.IsTypeValidateable(c))
                {
                    continue;
                }
                var nameSpace = c.Namespace;
                if (string.IsNullOrEmpty(nameSpace))
                {
                    result.Add($"Class {c.Name} has no declared namespace!", c, c);
                }
            }
        }
    }
}