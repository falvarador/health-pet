public class Pet
{
    public Pet()
    {
        Name = string.Empty;
        Breed = string.Empty;
    }

    public int PetId { get; set; }
    public int PetTypeId { get; set; }
    public string Name { get; set; }
    public Int16 Age { get; set; }
    public string Breed { get; set; }
}
