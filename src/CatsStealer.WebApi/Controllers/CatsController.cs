using CatStealer.Application.DTOs;
using CatStealer.Application.Models;
using CatStealer.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CatsStealer.WebApi.Controllers
{
    [ApiController]
    [Route("api/cats")]
    public class CatsController : ControllerBase
    {
        private readonly ICatsStealerService _catService;

        public CatsController(ICatsStealerService catService)
        {
            _catService = catService;
        }

        [HttpPost("fetch")]
        public async Task<IActionResult> FetchCats()
        {
            var fetchedCats = await _catService.FetchAndSaveCatsAsync();
            return Ok($"Fetched {fetchedCats} new cats.");
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CatDto>> GetCat(int id)
        {
            var cat = await _catService.GetCatByIdAsync(id);

            if (cat == null)
            {
                return NotFound();
            }

            return cat;
        }

        [HttpGet]
        public async Task<ActionResult<CatsResponse>> GetCats([FromQuery] string? tag = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var catsResponse = await _catService.GetCatsAsync(tag, page, pageSize);

            return Ok(catsResponse);
        }
    }
}
