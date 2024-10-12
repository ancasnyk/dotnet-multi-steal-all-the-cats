using CatStealer.Core.Entities;

namespace CatStealer.Core.Results
{
    public class CatsResult
    {
        public IEnumerable<CatEntity> Cats { get; set; }
        public int TotalCount { get; set; }

        public int TotalPages { get; set; }
    }
}
