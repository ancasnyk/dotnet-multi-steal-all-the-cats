using CatStealer.Core.Entities;
using CatStealer.Core.Results;

namespace CatStealer.Core.Interfaces.Repositories
{
    public interface ICatRepository
    {
        Task<CatEntity> GetCatByIdAsync(int id);
        Task<CatsResult> GetCatsAsync(string tag, int page, int pageSize);
        Task<TagEntity> GetOrCreateTagAsync(string tagName);
        Task SaveChangesAsync();
        Task<HashSet<string>> GetExistingCatIdsAsync(IEnumerable<string> catIds);
        Task AddCatsAsync(IEnumerable<CatEntity> cats);
    }
}
