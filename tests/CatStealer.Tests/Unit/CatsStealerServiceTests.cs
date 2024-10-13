using System.Runtime.InteropServices.JavaScript;
using CatStealer.Core.Entities;
using CatStealer.Core.Interfaces.Repositories;
using CatStealer.Core.Results;
using CatStealer.Infrastructure.Services;
using Moq;
using AutoMapper;
using CatStealer.Application.DTOs;
using CatStealer.Application.Models;
using CatStealer.Application.Services;

namespace CatStealer.Tests.Unit
{
    public class CatsStealerServiceTests
    {
        private readonly Mock<ICatRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ICatApiClient> _mockApiClient;

        public CatsStealerServiceTests()
        {
            _mockRepository = new Mock<ICatRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockApiClient = new Mock<ICatApiClient>();
        }

        [Fact]
        public async Task FetchAndSaveCatsAsync_ShouldReturnNumberOfFetchedCats()
        {
            // Arrange
            var apiResponses = new List<CatApiResponse>
            {
                new() { Id = "test1", Width = 100, Height = 100, Url = "https://example.com/cat1.jpg" },
                new() { Id = "test2", Width = 200, Height = 200, Url = "https://example.com/cat2.jpg" }
            };
            _mockApiClient.Setup(x => x.FetchCatsAsync()).ReturnsAsync(apiResponses);
            _mockApiClient.Setup(x => x.FetchImageAsync(It.IsAny<string>())).ReturnsAsync(new byte[] { 1, 2, 3, 4, 5 });
            _mockRepository.Setup(x => x.GetExistingCatIdsAsync(It.IsAny<IEnumerable<string>>())).ReturnsAsync(new HashSet<string>());

            var service = new CatsStealerService(_mockRepository.Object, _mockMapper.Object, _mockApiClient.Object);

            // Act
            var result = await service.FetchAndSaveCatsAsync();

            // Assert
            Assert.Equal(2, result);
            _mockRepository.Verify(x => x.AddCatsAsync(It.Is<IEnumerable<CatEntity>>(cats => cats.Count() == 2)), Times.Once);
        }

        [Fact]
        public async Task GetCatByIdAsync_ShouldReturnMappedCatDto()
        {
            // Arrange
            var catEntity = new CatEntity { Id = 1, CatId = "test1", Width = 100, Height = 100 };
            var catDto = new CatDto { Id = 1, CatId = "test1", Width = 100, Height = 100 };
            _mockRepository.Setup(x => x.GetCatByIdAsync(1)).ReturnsAsync(catEntity);
            _mockMapper.Setup(x => x.Map<CatDto>(catEntity)).Returns(catDto);

            var service = new CatsStealerService(_mockRepository.Object, _mockMapper.Object, _mockApiClient.Object);

            // Act
            var result = await service.GetCatByIdAsync(1);

            // Assert
            Assert.Equal(catDto, result);
        }

        [Fact]
        public async Task GetCatsAsync_ShouldReturnMappedCatsResponse()
        {
            // Arrange
            var catsResult = new CatsResult
            {
                Cats = new List<CatEntity> { new() { Id = 1, CatId = "test1" } },
                TotalCount = 1,
                TotalPages = 1
            };
            var mappedCatsResponse = new CatsResponse
            {
                Cats = new List<CatDto> { new() { Id = 1, CatId = "test1" } },
                TotalCount = 1,
                TotalPages = 1
            };
            _mockRepository.Setup(x => x.GetCatsAsync(null, 1, 10)).ReturnsAsync(catsResult);
            _mockMapper.Setup(x => x.Map<CatsResult, CatsResponse>(It.IsAny<CatsResult>())).Returns(mappedCatsResponse);

            var service = new CatsStealerService(_mockRepository.Object, _mockMapper.Object, _mockApiClient.Object);

            // Act
            var result = await service.GetCatsAsync(null, 1, 10);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(mappedCatsResponse.TotalCount, result.TotalCount);
            Assert.Equal(mappedCatsResponse.TotalPages, result.TotalPages);
            Assert.Equal(mappedCatsResponse.Cats.Count(), result.Cats.Count());
            Assert.Equal(mappedCatsResponse.Cats.First().Id, result.Cats.First().Id);
            Assert.Equal(mappedCatsResponse.Cats.First().CatId, result.Cats.First().CatId);
        }
    }
}