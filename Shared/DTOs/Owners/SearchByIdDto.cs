using System.ComponentModel.DataAnnotations;

public class SearchByIdDto
{
    public SearchByIdDto()
    {
        ID = string.Empty;
    }

    [Required, MinLength(9), MaxLength(50)]
    public string ID { get; set; }
}
