namespace LazySloth.Validation
{
    public abstract class BaseValidation 
    {
        protected abstract void Validate(ValidationResult result, string startPath);
    }
}