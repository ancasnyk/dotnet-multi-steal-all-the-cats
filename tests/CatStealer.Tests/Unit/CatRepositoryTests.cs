using CatStealer.Core.Entities;
using CatStealer.Infrastructure.Data;
using CatStealer.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CatStealer.Tests.Unit
{
    public class CatRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;

        public CatRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task GetCatByIdAsync_ShouldReturnCat_WhenCatExists()
        {
            // Arrange
            await using (var context = new ApplicationDbContext(_options))
            {
                var cat = new CatEntity { Id = 1, CatId = "test1", Width = 100, Height = 100, Image = new byte[] { } };
                context.Cats.Add(cat);
                await context.SaveChangesAsync();
            }

            // Act
            await using (var context = new ApplicationDbContext(_options))
            {
                var repository = new CatRepository(context);
                var result = await repository.GetCatByIdAsync(1);

                // Assert
                Assert.NotNull(result);
                Assert.Equal("test1", result.CatId);
            }
        }

        [Fact]
        public async Task GetCatsAsync_ShouldReturnPaginatedResult()
        {
            // Arrange
            await using (var context = new ApplicationDbContext(_options))
            {
                for (var i = 0; i < 20; i++)
                {
                    context.Cats.Add(new CatEntity { CatId = $"test{i}", Width = 100, Height = 100, Image = new byte[] { } });
                }
                await context.SaveChangesAsync();
            }

            // Act
            await using (var context = new ApplicationDbContext(_options))
            {
                var repository = new CatRepository(context);
                var result = await repository.GetCatsAsync(null, 1, 10);

                // Assert
                Assert.Equal(10, result.Cats.Count());
                Assert.Equal(20, result.TotalCount);
            }
        }

        [Fact]
        public async Task GetOrCreateTagAsync_ShouldCreateNewTag_WhenTagDoesNotExist()
        {
            // Arrange
            await using var context = new ApplicationDbContext(_options);
            var repository = new CatRepository(context);

            // Act
            var result = await repository.GetOrCreateTagAsync("NewTag");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("NewTag", result.Name);
            Assert.True(context.Tags.Any(t => t.Name == "NewTag"));
        }

        [Fact]
        public async Task GetExistingCatIdsAsync_ShouldReturnExistingIds()
        {
            // Arrange
            await using (var context = new ApplicationDbContext(_options))
            {
                context.Cats.AddRange(
                    new CatEntity { CatId = "exist1", Image = new byte[] { } },
                    new CatEntity { CatId = "exist2", Image = new byte[] { } }
                );

                await context.SaveChangesAsync();
            }

            // Act
            await using (var context = new ApplicationDbContext(_options))
            {
                var repository = new CatRepository(context);
                var result = await repository.GetExistingCatIdsAsync(new[] { "exist1", "exist2", "nonexist" });

                // Assert
                Assert.Equal(2, result.Count);
                Assert.Contains("exist1", result);
                Assert.Contains("exist2", result);
                Assert.DoesNotContain("nonexist", result);
            }
        }

        [Fact]
        public async Task AddCatsAsync_ShouldAddMultipleCats()
        {
            // Arrange
            var catsToAdd = new List<CatEntity>
            {
                new() { CatId = "new1", Width = 100, Height = 100, Image = new byte[]{} },
                new() { CatId = "new2", Width = 200, Height = 200, Image = new byte[]{} }
            };

            // Act
            await using (var context = new ApplicationDbContext(_options))
            {
                var repository = new CatRepository(context);
                await repository.AddCatsAsync(catsToAdd);
            }

            // Assert
            await using (var context = new ApplicationDbContext(_options))
            {
                Assert.Equal(2, await context.Cats.CountAsync());
                Assert.True(await context.Cats.AnyAsync(c => c.CatId == "new1"));
                Assert.True(await context.Cats.AnyAsync(c => c.CatId == "new2"));
            }
        }
    }
}
