namespace CleanHr.Domain;

public static class DepartmentDomainErrors
{
    public static string NameNullOrEmpty => "The DepartmentName value cannot be null or empty.";

    public static string DescriptionNullOrEmpty => "The Department description cannot be null or empty.";

    public static string GetDescriptionLengthOutOfRangeMessage(int minLength, int maxLength)
    {
        return $"The Department description must be in between {minLength} and {maxLength} characters.";
    }

    public static string GetNameLengthOutOfRangeErrorMessage(int minLength, int maxLength)
    {
        return $"The DepartmentName value must be in between {minLength} to {maxLength} characters.";
    }
}
