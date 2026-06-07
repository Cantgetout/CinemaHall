using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaHub.Data.Models
{
    public class MovieActor
    {
        [ForeignKey(nameof(Movie))] //Vmesto "Movie", se polzva imeto na klasa. Taka ako nqkoi den go preimenuvam Visual Studio avtomatichno shte go preimenuva i tuk
        public int MovieId { get; set; }
        public virtual Movie Movie { get; set; } = null!;

        [ForeignKey(nameof(Actor))]
        public int ActorId { get; set; }
        public virtual Actor Actor { get; set; } = null!;
    }
}