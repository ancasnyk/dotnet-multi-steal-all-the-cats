using CatStealer.Application.Services;
using CatStealer.Core.Interfaces.Repositories;
using CatStealer.Infrastructure.Data;
using CatStealer.Infrastructure.Repositories;
using CatStealer.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Microsoft.EntityFrameworkCore;
using CatStealer.Application.DTOs;
using CatStealer.Application.Models;
using CatStealer.Core.Entities;
using CatStealer.Core.Results;
using AutoMapper;

namespace CatStealer.Tests.Integration
{
    public class CatsStealerServiceIntegrationTests : IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ApplicationDbContext _dbContext;
        private readonly Mock<ICatApiClient> _mockCatApiClient;

        public CatsStealerServiceIntegrationTests()
        {
            var services = new ServiceCollection();

            // Set up in-memory database
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()));

            // Set up AutoMapper
            services.AddAutoMapper(typeof(CatsStealerService).Assembly);

            // Set up repository
            services.AddScoped<ICatRepository, CatRepository>();

            // Mock Cat API Client
            _mockCatApiClient = new Mock<ICatApiClient>();
            _mockCatApiClient.Setup(x => x.FetchCatsAsync()).ReturnsAsync(new List<CatApiResponse>
            {
                new CatApiResponse { Id = "test1", Width = 100, Height = 100, Url = "https://example.com/cat1.jpg" },
                new CatApiResponse { Id = "test2", Width = 200, Height = 200, Url = "https://example.com/cat2.jpg" }
            });
            _mockCatApiClient.Setup(x => x.FetchImageAsync(It.IsAny<string>())).ReturnsAsync(new byte[] { 1, 2, 3, 4, 5 });
            services.AddSingleton(_mockCatApiClient.Object);

            // Set up CatsStealerService
            services.AddScoped<ICatsStealerService, CatsStealerService>();

            _serviceProvider = services.BuildServiceProvider();

            _dbContext = _serviceProvider.GetRequiredService<ApplicationDbContext>();
        }

        [Fact]
        public async Task FetchAndSaveCatsAsync_ShouldSaveCatsToDatabase()
        {
            var service = _serviceProvider.GetRequiredService<ICatsStealerService>();

            var result = await service.FetchAndSaveCatsAsync();

            Assert.Equal(2, result);
            Assert.Equal(2, await _dbContext.Cats.CountAsync());
        }

        [Fact]
        public async Task GetCatByIdAsync_ShouldReturnSavedCat()
        {
            var service = _serviceProvider.GetRequiredService<ICatsStealerService>();
            await service.FetchAndSaveCatsAsync(); // Ensure cats are in the database

            var cat = await service.GetCatByIdAsync(1);

            Assert.NotNull(cat);
            Assert.Equal("test1", cat.CatId);
        }

        [Fact]
        public async Task GetCatsAsync_ShouldReturnPaginatedResults()
        {
            var service = _serviceProvider.GetRequiredService<ICatsStealerService>();
            await service.FetchAndSaveCatsAsync(); // Ensure cats are in the database

            var result = await service.GetCatsAsync(null, 1, 1);

            Assert.Single(result.Cats);
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.TotalPages);
        }

        [Fact]
        public async Task GetCatsAsync_ShouldFilterByTag_WhenTagIsProvided()
        {
            var service = _serviceProvider.GetRequiredService<ICatsStealerService>();
            var mapper = _serviceProvider.GetRequiredService<IMapper>();

            // Add test data to the in-memory database
            var tag = new TagEntity { Name = "Playful" };
            var cat = new CatEntity
            {
                CatId = "test1",
                Width = 100,
                Height = 100,
                Created = DateTime.UtcNow,
                Image = new byte[] { 1, 2, 3, 4, 5 },
                Tags = new List<TagEntity> { tag }
            };
            _dbContext.Tags.Add(tag);
            _dbContext.Cats.Add(cat);
            await _dbContext.SaveChangesAsync();

            var result = await service.GetCatsAsync("Playful", 1, 10);

            Assert.NotNull(result);
            Assert.Single(result.Cats);
            Assert.Equal(1, result.TotalCount);
            Assert.Equal(1, result.TotalPages);
            Assert.Equal("test1", result.Cats.First().CatId);
            Assert.Single(result.Cats.First().Tags);
            Assert.Equal("Playful", result.Cats.First().Tags.First().Name);
        }

        [Fact]
        public async Task FetchAndSaveCatsAsync_ShouldNotSaveDuplicateCats()
        {
            var service = _serviceProvider.GetRequiredService<ICatsStealerService>();

            // First, add a cat to the database
            _dbContext.Cats.Add(new CatEntity
            {
                CatId = "test1",
                Width = 100,
                Height = 100,
                Created = DateTime.UtcNow,
                Image = new byte[] { 1, 2, 3, 4, 5 }
            });
            await _dbContext.SaveChangesAsync();

            var result = await service.FetchAndSaveCatsAsync();

            Assert.Equal(1, result); // Only one new cat should be saved
            Assert.Equal(2, await _dbContext.Cats.CountAsync()); // Total count should be 2

            var savedCats = await _dbContext.Cats.ToListAsync();
            Assert.Contains(savedCats, c => c.CatId == "test1");
            Assert.Contains(savedCats, c => c.CatId == "test2");
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }
    }
}
