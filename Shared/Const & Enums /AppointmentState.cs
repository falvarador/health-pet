public class AppointmentState
{
    public const string Cancelled = "Cancelled";
    public const string Completed = "Completed";
    public const string Confirmed = "Confirmed";
    public const string Missing = "Missing";

    public static string ToFormatStateName(string state)
    {
        switch (state)
        {
            case Cancelled:
                return "Cancelada";
            case Completed:
                return "Completada";
            case Confirmed:
                return "Agendada";
            case Missing:
                return "Perdida";
            default:
                return string.Empty;
        }
    }
}
