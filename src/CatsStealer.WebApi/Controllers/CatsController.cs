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

        /// <summary>
        /// Endpoint used to fetch cats from the API and save them to the database.
        /// </summary>
        /// <returns><see cref="OkResult"/></returns>
        [HttpPost("fetch")]
        public async Task<IActionResult> FetchCats()
        {
            var fetchedCats = await _catService.FetchAndSaveCatsAsync();
            return Ok($"Fetched {fetchedCats} new cats.");
        }

        /// <summary>
        /// Get a cat by its ID.
        /// </summary>
        /// <param name="id">The cat ID</param>
        /// <returns>The <see cref="CatDto"/>.</returns>
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

        /// <summary>
        /// Get cats with optional tag, page, and page size parameters.
        /// </summary>
        /// <param name="tag">The tag name.</param>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns>Cat data</returns>
        [HttpGet]
        public async Task<ActionResult<CatsResponse>> GetCats([FromQuery] string? tag = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var catsResponse = await _catService.GetCatsAsync(tag, page, pageSize);

            return Ok(catsResponse);
        }
    }
}
