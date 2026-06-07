using System.ComponentModel.DataAnnotations;

namespace CinemaHub.Data.Models // Забележи namespace-а
{
    public class Movie
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = null!;

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = null!;    //oznachava che nqma da e null tova pole NIKOGA. Obeshtavash na kompilatora che nqma da e null kogato go polzvash, zashtoto EF shte go populni

        public int Year { get; set; }
        public string? ImageUrl { get; set; }

        // Връзка към Genre (One-to-Many)
        public int GenreId { get; set; }        //Tezi dva reda pozvolqvat na EF avtomatichno da zaredi i populni drugiq model bez da se pravqt manually sql zaqvki
        public virtual Genre Genre { get; set; } = null!;       //virtual e za da dopusne lazy loading

        // Vruzka kum Actors (Many-to-Many)
        public virtual HashSet<MovieActor> MoviesActors { get; set; } = new HashSet<MovieActor>();  //tova e mapping za many-to-many relationship v bazata
    }
}










/*
 1. Ролята на класа (Mapping Table / Join Entity)
Представи си го като "Договор".

Актьорът (Actor) съществува сам по себе си.

Филмът (Movie) съществува сам по себе си.

MovieActor е записът, който казва: "Актьорът Иван играе във филма Матрицата".*/