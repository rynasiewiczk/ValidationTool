namespace LazySloth.Validation
{
    using System.Collections.Generic;

    public abstract class BaseValidation 
    {
        protected abstract void Validate(ValidationResult result, string startPath, List<string> ignorePaths);
    }
}