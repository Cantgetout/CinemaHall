using System.ComponentModel.DataAnnotations;

namespace CinemaHub.Data.Models
{
    public class Actor
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = null!;

        public string? Bio { get; set; }

        public virtual ICollection<MovieActor> MoviesActors { get; set; } = new HashSet<MovieActor>();
    }
}