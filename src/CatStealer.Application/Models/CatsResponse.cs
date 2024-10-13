using CatStealer.Application.DTOs;

namespace CatStealer.Application.Models
{
    public class CatsResponse
    {
        public IEnumerable<CatDto> Cats { get; set; }
        public int TotalCount { get; set; }

        public int TotalPages { get; set; }
    }
}
