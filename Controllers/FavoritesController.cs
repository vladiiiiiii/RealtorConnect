using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealtorConnect.Models;
using RealtorConnect.Services.Interfaces;
using System.Security.Claims;

namespace RealtorConnect.Controllers
{
    [Route("api/Clients/{clientId}/[controller]")]
    [ApiController]
    [Authorize(Roles = "Client")]
    public class FavoritesController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;

        public FavoritesController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        [HttpPost("{apartmentId}")]
        public async Task<IActionResult> AddToFavorites(int clientId, int apartmentId)
        {
            // Проверяем, что clientId совпадает с ID авторизованного пользователя
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (clientId != userId)
                return Unauthorized("You can only manage your own favorites");

            await _favoriteService.AddToFavoritesAsync(clientId, apartmentId);
            return Ok(new { Message = "Apartment added to favorites" });
        }

        [HttpDelete("{apartmentId}")]
        public async Task<IActionResult> RemoveFromFavorites(int clientId, int apartmentId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (clientId != userId)
                return Unauthorized("You can only manage your own favorites");

            await _favoriteService.RemoveFromFavoritesAsync(clientId, apartmentId);
            return Ok(new { Message = "Apartment removed from favorites" });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Favorite>>> GetFavorites(int clientId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (clientId != userId)
                return Unauthorized("You can only view your own favorites");

            var favorites = await _favoriteService.GetFavoritesAsync(clientId);
            return Ok(favorites);
        }

        [HttpGet("{apartmentId}/is-favorite")]
        public async Task<ActionResult<bool>> IsFavorite(int clientId, int apartmentId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (clientId != userId)
                return Unauthorized("You can only check your own favorites");

            var isFavorite = await _favoriteService.IsFavoriteAsync(clientId, apartmentId);
            return Ok(isFavorite);
        }
    }
}