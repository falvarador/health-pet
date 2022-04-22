public class PetWithDescriptionDto
{
    public PetWithDescriptionDto()
    {
        Name = string.Empty;
        Breed = string.Empty;
        PetTypeDescription = string.Empty;
        IDOwner = string.Empty;
    }

    public int PetId { get; set; }
    public string Name { get; set; }
    public Int16 Age { get; set; }
    public string Breed { get; set; }
    public int PetTypeId { get; set; }
    public string PetTypeDescription { get; set; }
    public string IDOwner { get; set; }
}
