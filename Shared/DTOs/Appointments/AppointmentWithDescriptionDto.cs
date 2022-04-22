public class AppointmentWithDescriptionDto
{
    public AppointmentWithDescriptionDto()
    {
        OwnerDescription = string.Empty;
        CategoryDescription = string.Empty;
        Hour = string.Empty;
        State = string.Empty;
    }

    public int AppointmentId { get; set; }
    public int OwnerId { get; set; }
    public string OwnerDescription { get; set; }
    public int CategoryId { get; set; }
    public string CategoryDescription { get; set; }
    public string Hour { get; set; }
    public DateTime Date { get; set; }
    public string State { get; set; }
}
