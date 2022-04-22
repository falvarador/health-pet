using System.ComponentModel.DataAnnotations;

public class PetDto
{
    public PetDto()
    {
        Name = string.Empty;
        Breed = string.Empty;
        IDOwner = string.Empty;
    }

    [RequiredInteger]
    public int PetTypeId { get; set; }
    [Required, MaxLength(200)]
    public string Name { get; set; }
    [Required, Range(0, 30)]
    public Int16 Age { get; set; }
    [Required, MaxLength(150)]
    public string Breed { get; set; }
    public string IDOwner { get; set; }
}
