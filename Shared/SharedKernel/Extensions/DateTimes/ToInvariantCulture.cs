public static class ToInvariantCultureExtension
{
    public static string ToInvariantCulture(this DateTime dateTime)
    {
        return dateTime.ToString("yyyy-MM-dd");
    }
}
