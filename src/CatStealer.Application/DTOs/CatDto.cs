namespace CatStealer.Application.DTOs
{
    public class CatDto
    {
        public int Id { get; set; }
        public string CatId { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public DateTime Created { get; set; }
        public List<TagDto> Tags { get; set; }
    }
}
