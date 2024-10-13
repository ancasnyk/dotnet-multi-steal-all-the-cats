
namespace CatStealer.Core.Entities
{
    /// <summary>
    /// The entity model for a tag.
    /// </summary>
    public class TagEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }

        public ICollection<CatEntity> Cats { get; set; }
    }
}
