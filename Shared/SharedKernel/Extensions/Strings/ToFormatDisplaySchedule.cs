public static class ToFormatDisplayScheduleExtension
{
    public static string ToFormatDisplaySchedule(this string schedule)
    {
        switch (schedule)
        {
            case "08:00":
                return "8:00 AM";
            case "09:00":
                return "9:00 AM";
            case "10:00":
                return "10:00 AM";
            case "11:00":
                return "11:00 AM";
            case "13:00":
                return "1:00 PM";
            case "14:00":
                return "2:00 PM";
            case "15:00":
                return "3:00 PM";
            case "16:00":
                return "4:00 PM";
            default:
                return string.Empty;
        }
    }
}
