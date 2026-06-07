using System.ComponentModel.DataAnnotations;

namespace CinemaHub.Data.Models
{
    public class Genre
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = null!;

        public virtual ICollection<Movie> Movies { get; set; } = new HashSet<Movie>();
    }
}