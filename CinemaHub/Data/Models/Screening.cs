using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaHub.Data.Models
{
    public class Screening
    {
        public int Id { get; set; }

        // Vreme na projekciqta
        [Required]
        public DateTime StartDateTime { get; set; }

        // Film na projekciqta (Vruzka -> 1:n)
        [ForeignKey(nameof(Movie))]
        public int MovieId { get; set; }
        public virtual Movie Movie { get; set; } = null!;

        //Zala na projekciqta
        [ForeignKey(nameof(CinemaHall))]
        public int CinemaHallId { get; set; }
        public virtual CinemaHall CinemaHall { get; set; } = null!;
    }
}
