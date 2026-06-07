using System.ComponentModel.DataAnnotations;

namespace CinemaHub.Data.Models
{
    public class CinemaHall
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = null!;
        public int Capacity { get; set; }
    }
}