using System.ComponentModel.DataAnnotations;

public class AppointmentDto
{
    public AppointmentDto()
    {
        IDOwner = string.Empty;
        Hour = string.Empty;
        State = string.Empty;
    }

    public int AppointmentId { get; set; }
    public int OwnerId { get; set; }
    public string IDOwner { get; set; }
    [RequiredInteger]
    public int PetId { get; set; }
    [RequiredInteger]
    public int CategoryId { get; set; }
    [Required]
    public string Hour { get; set; }
    [Required]
    public DateTime Date { get; set; }
    public string State { get; set; }
}
