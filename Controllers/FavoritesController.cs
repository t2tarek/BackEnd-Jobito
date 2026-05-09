using System.Security.Claims;
using CompanyDashboardAPI.Data;
using CompanyDashboardAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CompanyDashboardAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FavoritesController : ControllerBase
{
    private readonly AppDbContext _context;

    public FavoritesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyFavorites()
    {
        var userId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var favorites = await _context.Favorites
            .Where(f => f.UserId == userId)
            .Select(f => f.JobId)
            .ToListAsync();

        return Ok(favorites);
    }

    [HttpPost("toggle/{jobId}")]
    public async Task<IActionResult> ToggleFavorite(long jobId)
    {
        var userId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var existingFav = await _context.Favorites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.JobId == jobId);

        if (existingFav != null)
        {
            _context.Favorites.Remove(existingFav);
            await _context.SaveChangesAsync();
            return Ok(new { isFavorite = false });
        }
        else
        {
            var fav = new Favorite
            {
                UserId = userId,
                JobId = jobId,
                CreatedAt = DateTime.UtcNow
            };
            _context.Favorites.Add(fav);
            await _context.SaveChangesAsync();
            return Ok(new { isFavorite = true });
        }
    }
}
