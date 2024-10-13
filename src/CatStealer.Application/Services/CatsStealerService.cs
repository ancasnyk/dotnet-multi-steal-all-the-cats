using AutoMapper;
using CatStealer.Application.DTOs;
using CatStealer.Application.Models;
using CatStealer.Core.Entities;
using CatStealer.Core.Interfaces.Repositories;
using CatStealer.Core.Results;
using CatStealer.Infrastructure.Services;

namespace CatStealer.Application.Services
{
    /// <summary>
    /// The service for fetching and saving cats.
    /// </summary>
    public interface ICatsStealerService
    {
        /// <summary>
        /// Downloads and stores cat data.
        /// </summary>
        /// <returns>The number of unique cats downloaded.</returns>
        Task<int> FetchAndSaveCatsAsync();

        /// <summary>
        /// Get a cat by its ID.
        /// </summary>
        /// <param name="id">The ID.</param>
        /// <returns>The Cat.</returns>
        Task<CatDto> GetCatByIdAsync(int id);

        /// <summary>
        /// Get cats with optional tag filtering and paging.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns>The cats.</returns>
        Task<CatsResponse> GetCatsAsync(string? tag, int page, int pageSize);
    }

    /// <inheritdoc />
    public class CatsStealerService : ICatsStealerService
    {
        private readonly ICatRepository _catRepository;
        private readonly IMapper _mapper;
        private readonly ICatApiClient _catApiClient;

        public CatsStealerService(ICatRepository catRepository,
            IMapper mapper,
            ICatApiClient catApiClient)
        {
            _catRepository = catRepository;
            _mapper = mapper;
            _catApiClient = catApiClient;
        }

        /// <inheritdoc />
        public async Task<int> FetchAndSaveCatsAsync()
        {
            var newCats = new List<CatEntity>();
            var fetchedCats = 0;

            var cats = await _catApiClient.FetchCatsAsync();
            
            // Get all cat IDs from the API responses
            var catIds = cats.Select(c => c.Id).ToList();

            // Check which cats already exist in the database.
            var existingCatIds = await _catRepository.GetExistingCatIdsAsync(catIds);

            foreach (var catData in cats)
            {
                var catAlreadyExists = existingCatIds.Contains(catData.Id);

                if (catAlreadyExists == false)
                {
                    var cat = new CatEntity
                    {
                        CatId = catData.Id,
                        Width = catData.Width,
                        Height = catData.Height,
                        Created = DateTime.UtcNow,
                        // TODO; Should really be using a Storage Provider and store the result of the save operation,
                        // but for the sake of this exercise, we'll just fetch the image and just store the content in the DB.
                        Image = await _catApiClient.FetchImageAsync(catData.Url)
                    };

                    if (catData.Breeds?.Count > 0)
                    {
                        var temperaments = catData.Breeds[0].Temperament.Split(',');
                        foreach (var temperament in temperaments)
                        {
                            // TODO: Optimize this further by checking which tags already exist in the database.
                            var tag = await _catRepository.GetOrCreateTagAsync(temperament.Trim());
                            cat.Tags ??= new List<TagEntity>();
                            cat.Tags.Add(tag);
                        }
                    }

                    newCats.Add(cat);
                    fetchedCats++;
                }
            }

            // Bulk insert new cats
            if (newCats.Any())
            {
                await _catRepository.AddCatsAsync(newCats);
            }

            return fetchedCats;
        }

        /// <inheritdoc />
        public async Task<CatDto> GetCatByIdAsync(int id)
        {
            var cat = await _catRepository.GetCatByIdAsync(id);

            return _mapper.Map<CatDto>(cat);
        }

        /// <inheritdoc />
        public async Task<CatsResponse> GetCatsAsync(string? tag, int page, int pageSize)
        {
            var catsResult = await _catRepository.GetCatsAsync(tag, page, pageSize);

            return _mapper.Map<CatsResult, CatsResponse>(catsResult);
        }
    }
}
