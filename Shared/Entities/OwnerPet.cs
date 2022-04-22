public class OwnerPet
{
    public OwnerPet()
    {
        IDOwner = string.Empty;
    }

    public int PetId { get; set; }
    public int OwnerId { get; set; }
    public string IDOwner { get; set; }
}
