using System.ComponentModel.DataAnnotations;

public class OwnerDto
{
    public OwnerDto()
    {
        Name = string.Empty;
        LastName = string.Empty;
        ID = string.Empty;
        Phone = string.Empty;
        Email = string.Empty;
    }

    public int OwnerId { get; set; }
    [Required, MaxLength(100)]
    public string Name { get; set; }
    [Required, MaxLength(200)]
    public string LastName { get; set; }
    [Required, MinLength(9), MaxLength(50)]
    public string ID { get; set; }
    [Required, MaxLength(25)]
    public string Phone { get; set; }
    [Required, EmailAddress]
    public string Email { get; set; }
}
