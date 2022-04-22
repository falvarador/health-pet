public class Appointment
{
    public Appointment()
    {
        Hour = string.Empty;
        State = string.Empty;
    }

    public int AppointmentId { get; set; }
    public int OwnerId { get; set; }
    public int PetId { get; set; }
    public int CategoryId { get; set; }
    public string Hour { get; set; }
    public DateTime Date { get; set; }
    public string State { get; set; }
}
