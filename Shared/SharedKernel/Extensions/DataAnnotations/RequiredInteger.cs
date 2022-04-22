using System.ComponentModel.DataAnnotations;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class RequiredIntegerAttribute : ValidationAttribute
{
    public RequiredIntegerAttribute()
        : base("The {0} field is required.")
    { }

    public override bool IsValid(object? value)
    {
        return Validate(value);
    }

    private bool Validate(object? value)
    {
        var integerValue = value as Nullable<int>;

        if (value == null || integerValue.GetValueOrDefault() <= 0)
            return false;

        return true;
    }
}
