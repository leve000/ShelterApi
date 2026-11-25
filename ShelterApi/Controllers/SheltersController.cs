using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShelterApi.Models;
using ShelterApi.Models.DTOs;

namespace ShelterApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SheltersController : ControllerBase
{
    private readonly AnimalshelterContext _context;

    public SheltersController(AnimalshelterContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var shelters = await _context.Shelters.AsNoTracking().ToListAsync();
            return Ok(new { message = "A menhelyek lekérdezése sikeres.", result = shelters });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "A menhelyek lekérdezése sikertelen.", result = (object?)null });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var shelter = await _context.Shelters.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
            if (shelter is null)
            {
                return NotFound(new { message = "A menhely nem található.", result = (object?)null });
            }

            return Ok(new { message = "A menhely lekérdezése sikeres.", result = shelter });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "A menhely lekérdezése sikertelen.", result = (object?)null });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create(NewShelterDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.City) || dto.Capacity < 0)
            {
                return BadRequest(new { message = "A menhely neve, városa és nem negatív kapacitása kötelező.", result = (object?)null });
            }

            if (await _context.Shelters.AnyAsync(s => s.Name == dto.Name))
            {
                return Conflict(new { message = "Ilyen nevű menhely már létezik.", result = (object?)null });
            }

            var shelter = new Shelter
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                City = dto.City,
                Capacity = dto.Capacity,
                RegTime = DateTime.Now
            };

            _context.Shelters.Add(shelter);
            await _context.SaveChangesAsync();
            return StatusCode(StatusCodes.Status201Created, new { message = "A menhely létrehozása sikeres.", result = shelter });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "A menhely létrehozása sikertelen.", result = (object?)null });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateShelterDto dto)
    {
        try
        {
            var shelter = await _context.Shelters.FirstOrDefaultAsync(s => s.Id == id);
            if (shelter is null)
            {
                return NotFound(new { message = "A menhely nem található.", result = (object?)null });
            }

            if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.City) || dto.Capacity < 0)
            {
                return BadRequest(new { message = "A menhely neve, városa és nem negatív kapacitása kötelező.", result = (object?)null });
            }

            if (await _context.Shelters.AnyAsync(s => s.Id != id && s.Name == dto.Name))
            {
                return Conflict(new { message = "Ilyen nevű menhely már létezik.", result = (object?)null });
            }

            shelter.Name = dto.Name;
            shelter.City = dto.City;
            shelter.Capacity = dto.Capacity;
            await _context.SaveChangesAsync();
            return Ok(new { message = "A menhely módosítása sikeres.", result = shelter });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "A menhely módosítása sikertelen.", result = (object?)null });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var shelter = await _context.Shelters.FirstOrDefaultAsync(s => s.Id == id);
            if (shelter is null)
            {
                return NotFound(new { message = "A menhely nem található.", result = (object?)null });
            }

            _context.Shelters.Remove(shelter);
            await _context.SaveChangesAsync();
            return Ok(new { message = "A menhely törlése sikeres.", result = shelter });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "A menhely törlése sikertelen.", result = (object?)null });
        }
    }
}
