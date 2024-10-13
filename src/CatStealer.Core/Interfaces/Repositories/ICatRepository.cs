using CatStealer.Core.Entities;
using CatStealer.Core.Results;

namespace CatStealer.Core.Interfaces.Repositories
{
    /// <summary>
    /// The repository for the cats.
    /// </summary>
    public interface ICatRepository
    {
        /// <summary>
        /// Get a cat by ID.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The cat.</returns>
        Task<CatEntity> GetCatByIdAsync(int id);

        /// <summary>
        /// Get cats with optional tag filtering and paging.
        /// </summary>
        /// <param name="tag">The tag name.</param>
        /// <param name="page">The page</param>
        /// <param name="pageSize">The page size</param>
        /// <returns>The cats.</returns>
        Task<CatsResult> GetCatsAsync(string tag, int page, int pageSize);

        /// <summary>
        /// Gets or Creates and returns the new tag.
        /// </summary>
        /// <param name="tagName">the Tag name</param>
        /// <returns>The new tag entity.</returns>
        Task<TagEntity> GetOrCreateTagAsync(string tagName);

        /// <summary>
        /// Saves changes to the database.
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        Task SaveChangesAsync();

        /// <summary>
        /// Get the existing cat IDs based on cat IDs input.
        /// </summary>
        /// <param name="catIds">A list of cat IDs</param>
        /// <returns>The cat IDs already existing in the DB.</returns>
        Task<HashSet<string>> GetExistingCatIdsAsync(IEnumerable<string> catIds);

        /// <summary>
        /// Add cats to the database.
        /// </summary>
        /// <param name="cats">The cats to add.</param>
        /// <returns><see cref="Task"/></returns>
        Task AddCatsAsync(IEnumerable<CatEntity> cats);
    }
}
