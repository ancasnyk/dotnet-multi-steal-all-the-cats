using CatStealer.Core.Entities;
using CatStealer.Core.Interfaces.Repositories;
using CatStealer.Core.Results;
using CatStealer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CatStealer.Infrastructure.Repositories
{
    /// <inheritdoc />
    public class CatRepository : ICatRepository
    {
        private readonly ApplicationDbContext _context;

        public CatRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <inheritdoc />
        public async Task<CatEntity> GetCatByIdAsync(int id)
        {
            return await _context.Cats
                .Include(c => c.Tags)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        /// <inheritdoc />
        public async Task<CatsResult> GetCatsAsync(string tag, int page, int pageSize)
        {
            IQueryable<CatEntity> query = _context.Cats.Include(c => c.Tags);

            if (string.IsNullOrEmpty(tag) == false)
            {
                query = query.Where(c => c.Tags.Any(t => t.Name == tag));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var cats = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new CatsResult
            {
                Cats = cats, 
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }

        /// <inheritdoc />
        public async Task<TagEntity> GetOrCreateTagAsync(string tagName)
        {
            var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName.Trim());
            if (tag == null)
            {
                tag = new TagEntity { Name = tagName.Trim(), Created = DateTime.UtcNow };
                _context.Tags.Add(tag);
                await _context.SaveChangesAsync();
            }
            return tag;
        }

        /// <inheritdoc />
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<HashSet<string>> GetExistingCatIdsAsync(IEnumerable<string> catIds)
        {
            return new HashSet<string>(
                await _context.Cats
                    .Where(c => catIds.Contains(c.CatId))
                    .Select(c => c.CatId)
                    .ToListAsync()
            );
        }

        /// <inheritdoc />
        public async Task AddCatsAsync(IEnumerable<CatEntity> cats)
        {
            await _context.Cats.AddRangeAsync(cats);
            await _context.SaveChangesAsync();
        }
    }
}
