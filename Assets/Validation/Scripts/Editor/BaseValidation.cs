namespace LazySloth.Validation
{
    using System.Collections.Generic;

    public abstract class BaseValidation 
    {
        public abstract void RunValidation(string startPath, List<string> ignorePaths, ValidationResult result = null);
        protected abstract void Validate(ValidationResult result, string startPath, List<string> ignorePaths);
    }
}