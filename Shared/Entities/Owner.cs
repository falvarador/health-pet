public class Owner
{
    public Owner()
    {
        Name = string.Empty;
        LastName = string.Empty;
        ID = string.Empty;
        Phone = string.Empty;
        Email = string.Empty;
    }

    public int OwnerId { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; }
    public string ID { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
}
