using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShelterApi.Models;
using ShelterApi.Models.DTOs;

namespace ShelterApi.Controllers;

[ApiController]
[Route("animals")]
public class AnimalsController : ControllerBase
{
    private readonly AnimalshelterContext _context;

    public AnimalsController(AnimalshelterContext context)
    {
        _context = context;
    }

    [HttpGet("allAnimal")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var animals = await _context.Animals.AsNoTracking().ToListAsync();
            if (animals.Count == 0)
            {
                return NotFound(new { message = "Nincs rögzített állat.", result = (object?)null });
            }

            return Ok(new { message = "Az állatok lekérdezése sikeres.", result = animals });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Az állatok lekérdezése sikertelen.", result = (object?)null });
        }
    }

    [HttpGet("byId")]
    public async Task<IActionResult> GetById([FromQuery] Guid id)
    {
        try
        {
            var animal = await _context.Animals.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
            if (animal is null)
            {
                return NotFound(new { message = "Az állat nem található.", result = (object?)null });
            }

            return Ok(new { message = "Az állat lekérdezése sikeres.", result = animal });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Az állat lekérdezése sikertelen.", result = (object?)null });
        }
    }

    [HttpPost("newAnimal")]
    public async Task<IActionResult> Create(NewAnimalDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Species))
            {
                return BadRequest(new { message = "Az állat neve és faja kötelező.", result = (object?)null });
            }

            if (dto.ShelterId.HasValue && !await _context.Shelters.AnyAsync(s => s.Id == dto.ShelterId.Value))
            {
                return NotFound(new { message = "A megadott menhely nem található.", result = (object?)null });
            }

            if (await _context.Animals.AnyAsync(a => a.Name == dto.Name && a.ShelterId == dto.ShelterId))
            {
                return Conflict(new { message = "Ilyen nevű állat már létezik ezen a menhelyen.", result = (object?)null });
            }

            var animal = new Animal
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Species = dto.Species,
                Age = dto.Age,
                Weight = dto.Weight,
                ShelterId = dto.ShelterId,
                RegTime = DateTime.Now
            };

            _context.Animals.Add(animal);
            await _context.SaveChangesAsync();
            return StatusCode(StatusCodes.Status201Created, new { message = "Az állat létrehozása sikeres.", result = animal });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Az állat létrehozása sikertelen.", result = (object?)null });
        }
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update([FromQuery] Guid id, UpdateAnimalDto dto)
    {
        try
        {
            var animal = await _context.Animals.FirstOrDefaultAsync(a => a.Id == id);
            if (animal is null)
            {
                return NotFound(new { message = "Az állat nem található.", result = (object?)null });
            }

            if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Species))
            {
                return BadRequest(new { message = "Az állat neve és faja kötelező.", result = (object?)null });
            }

            if (dto.ShelterId.HasValue && !await _context.Shelters.AnyAsync(s => s.Id == dto.ShelterId.Value))
            {
                return NotFound(new { message = "A megadott menhely nem található.", result = (object?)null });
            }

            if (await _context.Animals.AnyAsync(a => a.Id != id && a.Name == dto.Name && a.ShelterId == dto.ShelterId))
            {
                return Conflict(new { message = "Ilyen nevű állat már létezik ezen a menhelyen.", result = (object?)null });
            }

            animal.Name = dto.Name;
            animal.Species = dto.Species;
            animal.Age = dto.Age;
            animal.Weight = dto.Weight;
            animal.ShelterId = dto.ShelterId;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Az állat módosítása sikeres.", result = animal });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Az állat módosítása sikertelen.", result = (object?)null });
        }
    }

    [HttpDelete("deleteAnimal")]
    public async Task<IActionResult> Delete([FromQuery] Guid id)
    {
        try
        {
            var animal = await _context.Animals.FirstOrDefaultAsync(a => a.Id == id);
            if (animal is null)
            {
                return NotFound(new { message = "Az állat nem található.", result = (object?)null });
            }

            _context.Animals.Remove(animal);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Az állat törlése sikeres.", result = animal });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Az állat törlése sikertelen.", result = (object?)null });
        }
    }
}
