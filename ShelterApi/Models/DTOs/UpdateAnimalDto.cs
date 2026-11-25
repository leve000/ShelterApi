namespace ShelterApi.Models.DTOs
{
    public class UpdateAnimalDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Species { get; set; }
        public int? Age { get; set; }
        public double? Weight { get; set; }
        public Guid? ShelterId { get; set; }
        public DateTime? RegTime { get; set; }
    }
}
