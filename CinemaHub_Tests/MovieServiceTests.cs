using CinemaHub.Data;
using CinemaHub.Data.Models;
using CinemaHub.Data.Models.ViewModels;
using CinemaHub.Models.ViewModels;
using CinemaHub.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using NUnit.Framework.Legacy; // <--- ВАЖНО: Тук живее ClassicAssert
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaHub.Tests
{
    [TestFixture]
    public class MovieServiceTests
    {
        private ApplicationDbContext _context;
        private IMovieService _movieService;

        [SetUp]
        public void Setup()
        {
            // 1. Уникална база за всеки тест
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "MovieDb_" + Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            // 2. Сийдваме помощни данни
            _context.Genres.Add(new Genre { Id = 1, Name = "Action" });
            _context.Actors.Add(new Actor { Id = 10, Name = "Actor A", Bio = "Bio A" });
            _context.Actors.Add(new Actor { Id = 20, Name = "Actor B", Bio = "Bio B" });
            _context.SaveChanges();

            _movieService = new MovieService(_context);
        }

        [TearDown]
        public void Teardown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task AddMovieAsync_ShouldAddMovieWithActors()
        {
            // Arrange
            var model = new AddMovieViewModel
            {
                Title = "New Movie",
                Description = "Test Desc",
                Year = 2024,
                GenreId = 1,
                ImageUrl = "img.jpg",
                SelectedActorIds = new List<int> { 10, 20 }
            };

            // Act
            await _movieService.AddMovieAsync(model);

            // Assert
            var movie = await _context.Movies.Include(m => m.MoviesActors).FirstOrDefaultAsync();

            ClassicAssert.IsNotNull(movie);
            ClassicAssert.AreEqual("New Movie", movie.Title);
            ClassicAssert.AreEqual(2, movie.MoviesActors.Count, "Should have 2 actors linked.");
            // Проверяваме дали конкретния актьор е вързан
            ClassicAssert.IsTrue(movie.MoviesActors.Any(ma => ma.ActorId == 10));
        }

        [Test]
        public async Task GetAllMoviesAsync_ShouldReturnAllMovies()
        {
            // Arrange
            _context.Movies.Add(new Movie { Title = "M1", GenreId = 1, Description = "D", ImageUrl = "U", Year = 2020 });
            _context.Movies.Add(new Movie { Title = "M2", GenreId = 1, Description = "D", ImageUrl = "U", Year = 2021 });
            await _context.SaveChangesAsync();

            // Act
            var result = await _movieService.GetAllMoviesAsync();

            // Assert
            ClassicAssert.AreEqual(2, result.Movies.Count());
            ClassicAssert.AreEqual("M2", result.Movies.First().Title);
        }

        [Test]
        public async Task GetMovieForEditAsync_ShouldReturnModelWithSelectedActors()
        {
            // Arrange
            var movie = new Movie { Title = "Edit Me", GenreId = 1, Description = "D", ImageUrl = "U", Year = 2020 };
            await _context.Movies.AddAsync(movie);
            await _context.SaveChangesAsync();

            _context.MoviesActors.Add(new MovieActor { MovieId = movie.Id, ActorId = 10 });
            await _context.SaveChangesAsync();

            // Act
            var result = await _movieService.GetMovieForEditAsync(movie.Id);

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual("Edit Me", result.Title);
            ClassicAssert.AreEqual(1, result.SelectedActorIds.Count);
            ClassicAssert.AreEqual(10, result.SelectedActorIds.First());
        }

        [Test]
        public async Task GetMovieForEditAsync_ShouldReturnNull_WhenIdIsInvalid()
        {
            // Act
            var result = await _movieService.GetMovieForEditAsync(999);

            // Assert
            ClassicAssert.IsNull(result);
        }

        [Test]
        public async Task UpdateMovieAsync_ShouldUpdateDetailsAndChangeActors()
        {
            // Arrange
            var movie = new Movie { Title = "Old Title", GenreId = 1, Description = "D", ImageUrl = "U", Year = 2020 };
            await _context.Movies.AddAsync(movie);
            await _context.SaveChangesAsync();

            // Стара връзка с Actor 10
            _context.MoviesActors.Add(new MovieActor { MovieId = movie.Id, ActorId = 10 });
            await _context.SaveChangesAsync();

            var editModel = new EditMovieViewModel
            {
                Id = movie.Id,
                Title = "New Title",
                Description = "New Desc",
                Year = 2025,
                GenreId = 1,
                ImageUrl = "img2.jpg",
                // Сменяме актьорите: махаме 10, слагаме 20
                SelectedActorIds = new List<int> { 20 }
            };

            // Act
            await _movieService.UpdateMovieAsync(editModel);

            // Assert
            var updatedMovie = await _context.Movies.Include(m => m.MoviesActors).FirstOrDefaultAsync(m => m.Id == movie.Id);

            ClassicAssert.AreEqual("New Title", updatedMovie.Title);
            ClassicAssert.AreEqual(1, updatedMovie.MoviesActors.Count);
            ClassicAssert.AreEqual(20, updatedMovie.MoviesActors.First().ActorId);
        }

        [Test]
        public async Task DeleteMovieAsync_ShouldRemoveMovie()
        {
            // Arrange
            var movie = new Movie { Title = "Delete Me", GenreId = 1, Description = "D", ImageUrl = "U", Year = 2020 };
            await _context.Movies.AddAsync(movie);
            await _context.SaveChangesAsync();

            // Act
            await _movieService.DeleteMovieAsync(movie.Id);

            // Assert
            var count = await _context.Movies.CountAsync();
            ClassicAssert.AreEqual(0, count);
        }

        [Test]
        public async Task GetMovieDetailsAsync_ShouldReturnCorrectData()
        {
            // Arrange
            var movie = new Movie { Title = "Details Movie", GenreId = 1, Description = "D", ImageUrl = "U", Year = 2022 };
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            // Act
            var result = await _movieService.GetMovieDetailsAsync(movie.Id);

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual("Details Movie", result.Title);
            ClassicAssert.AreEqual("Action", result.Genre);
        }
    }
}