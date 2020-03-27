namespace LazySloth.Validation
{
    using System;
    using JetBrains.Annotations;

    [AttributeUsage(AttributeTargets.Method)] [MeansImplicitUse]
    public class ValidateMethodAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Field)]
    public class OptionalObjectFieldAttribute : Attribute
    {
        //public bool RequiredInScene = false;
    }
}