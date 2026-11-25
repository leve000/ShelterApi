namespace ShelterApi.Models.DTOs;

public class NewShelterDto
{
    public string? Name { get; set; }
    public string? City { get; set; }
    public int Capacity { get; set; }
}

public class UpdateShelterDto
{
    public string? Name { get; set; }
    public string? City { get; set; }
    public int Capacity { get; set; }
}
