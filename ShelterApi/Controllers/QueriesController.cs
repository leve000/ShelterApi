using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShelterApi.Models;

namespace ShelterApi.Controllers;

[ApiController]
[Route("queries")]
public class QueriesController : ControllerBase
{
    private readonly AnimalshelterContext _context;

    public QueriesController(AnimalshelterContext context)
    {
        _context = context;
    }

    [HttpPost("NewAnimal")]

    public async Task<IActionResult> CreateAnimal(NewAnimalDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Species) || dto.Age < 0)
            {
                return BadRequest(new { message = "Az állat neve, fajtája és nem negatív életkora kötelező.", result = (object?)null });
            }
            var animal = new Animal
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Species = dto.Species,
                Age = dto.Age
            };
            _context.Animals.Add(animal);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Az állat létrehozása sikeres.", result = animal });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Az állat létrehozása sikertelen.", result = (object?)null });
        }
    }
}
