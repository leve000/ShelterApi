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

    [HttpGet("allSheltersWithAnimals")]
    public async Task<IActionResult> AllSheltersWithAnimals()
    {
        try
        {
            var shelters = await _context.Shelters
                .Include(s => s.Animals)
                .AsNoTracking()
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.City,
                    s.Capacity,
                    s.RegTime,
                    Animals = s.Animals.Select(a => new
                    {
                        a.Id,
                        a.Name,
                        a.Species,
                        a.Age,
                        a.Weight,
                        a.RegTime,
                        a.ShelterId
                    }).ToList()
                })
                .ToListAsync();

            if (shelters.Count == 0)
            {
                return NotFound(new { message = "Nincs rögzített menhely.", result = (object?)null });
            }

            return Ok(new { message = "A menhelyek és állataik lekérdezése sikeres.", result = shelters });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "A menhelyek és állataik lekérdezése sikertelen.", result = (object?)null });
        }
    }

    [HttpGet("allAnimalsWithShelter")]
    public async Task<IActionResult> AllAnimalsWithShelter()
    {
        try
        {
            var animals = await _context.Animals
                .Join(
                    _context.Shelters,
                    animal => animal.ShelterId,
                    shelter => shelter.Id,
                    (animal, shelter) => new
                    {
                        animal.Id,
                        animal.Name,
                        animal.Species,
                        animal.Age,
                        animal.Weight,
                        animal.RegTime,
                        animal.ShelterId,
                        ShelterName = shelter.Name
                    })
                .AsNoTracking()
                .ToListAsync();

            if (animals.Count == 0)
            {
                return NotFound(new { message = "Nincs menhelyhez kapcsolódó állat.", result = (object?)null });
            }

            return Ok(new { message = "Az állatok és menhelyeik lekérdezése sikeres.", result = animals });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Az állatok és menhelyeik lekérdezése sikertelen.", result = (object?)null });
        }
    }

    [HttpGet("animalsOfShelter")]
    public async Task<IActionResult> AnimalsOfShelter([FromQuery] Guid shelterId)
    {
        try
        {
            var shelter = await _context.Shelters
                .Include(s => s.Animals)
                .AsNoTracking()
                .Where(s => s.Id == shelterId)
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.City,
                    s.Capacity,
                    s.RegTime,
                    Animals = s.Animals.Select(a => new
                    {
                        a.Id,
                        a.Name,
                        a.Species,
                        a.Age,
                        a.Weight,
                        a.RegTime,
                        a.ShelterId
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (shelter is null)
            {
                return NotFound(new { message = "A menhely nem található.", result = (object?)null });
            }

            return Ok(new { message = "A menhely és állatainak lekérdezése sikeres.", result = shelter });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "A menhely és állatainak lekérdezése sikertelen.", result = (object?)null });
        }
    }

    [HttpGet("animalCountByShelter")]
    public async Task<IActionResult> AnimalCountByShelter()
    {
        try
        {
            var counts = await _context.Shelters
                .AsNoTracking()
                .Select(s => new
                {
                    ShelterId = s.Id,
                    ShelterName = s.Name,
                    AnimalCount = s.Animals.Count()
                })
                .OrderByDescending(s => s.AnimalCount)
                .ToListAsync();

            if (counts.Count == 0)
            {
                return NotFound(new { message = "Nincs rögzített menhely.", result = (object?)null });
            }

            return Ok(new { message = "Az állatok menhelyenkénti darabszámának lekérdezése sikeres.", result = counts });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Az állatok menhelyenkénti darabszámának lekérdezése sikertelen.", result = (object?)null });
        }
    }
}
