using CinemaHub.Data;
using CinemaHub.Data.Models;
using CinemaHub.Data.Models.ViewModels;
using CinemaHub.Models.ViewModels;
using CinemaHub.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using NUnit.Framework.Legacy; // За ClassicAssert
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaHub.Tests
{
    [TestFixture]
    public class ScreeningServiceTests
    {
        private ApplicationDbContext _context;
        private IScreeningService _screeningService;

        [SetUp]
        public void Setup()
        {
            // 1. Създаваме уникална база за всеки тест
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "ScreeningDb_" + Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            // 2. ВАЖНО: Трябва да имаме Филм и Зала, за да създадем Прожекция
            _context.Movies.Add(new Movie
            {
                Id = 1,
                Title = "Test Movie",
                Description = "D",
                Year = 2024,
                ImageUrl = "url",
                GenreId = 1 // Предполагаме, че жанрът не е критичен за тези тестове
            });

            _context.CinemaHalls.Add(new CinemaHall
            {
                Id = 1,
                Name = "Grand Hall",
                Capacity = 100
            });

            _context.CinemaHalls.Add(new CinemaHall
            {
                Id = 2,
                Name = "VIP Hall",
                Capacity = 20
            });

            _context.SaveChanges();

            _screeningService = new ScreeningService(_context);
        }

        [TearDown]
        public void Teardown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task AddScreeningAsync_ShouldAddScreeningToDatabase()
        {
            // Arrange
            var model = new ScreeningFormModel
            {
                MovieId = 1,        
                CinemaHallId = 1,
                StartDateTime = DateTime.Now.AddDays(1)
            };

            // Act
            await _screeningService.AddScreeningAsync(model);

            // Assert
            var count = await _context.Screenings.CountAsync();
            ClassicAssert.AreEqual(1, count);

            var savedScreening = await _context.Screenings.FirstAsync();
            ClassicAssert.AreEqual(1, savedScreening.MovieId);
            ClassicAssert.AreEqual(1, savedScreening.CinemaHallId);
        }

        [Test]
        public async Task GetAllScreeningsAsync_ShouldReturnScreeningsWithRelatedData()
        {
            // Arrange
            var screening = new Screening
            {
                MovieId = 1,
                CinemaHallId = 1,
                StartDateTime = new DateTime(2025, 5, 20, 18, 30, 0)
            };
            await _context.Screenings.AddAsync(screening);
            await _context.SaveChangesAsync();

            // Act
            var result = await _screeningService.GetAllScreeningsAsync();

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(1, result.Count());

            var viewModel = result.First();
            
            //Includes
            ClassicAssert.AreEqual("Test Movie", viewModel.MovieTitle);
            ClassicAssert.AreEqual("Grand Hall", viewModel.HallName);
            ClassicAssert.AreEqual("20.05.2025 18:30", viewModel.StartDateTime);
        }

        [Test]
        public async Task GetScreeningFormModelAsync_ShouldLoadDropdownLists()
        {
            // Act
           
            var result = await _screeningService.GetScreeningFormModelAsync();

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.IsNotNull(result.Movies);
            ClassicAssert.AreEqual(1, result.Movies.Count());
            ClassicAssert.IsNotNull(result.Halls);
            ClassicAssert.AreEqual(2, result.Halls.Count());
        }

        [Test]
        public async Task GetScreeningForEditAsync_ShouldReturnCorrectModel()
        {
            // Arrange
            var screening = new Screening { MovieId = 1, CinemaHallId = 1, StartDateTime = DateTime.Now };
            await _context.Screenings.AddAsync(screening);
            await _context.SaveChangesAsync();

            // Act
            var result = await _screeningService.GetScreeningForEditAsync(screening.Id);

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(screening.Id, result.Id);
            ClassicAssert.AreEqual(1, result.MovieId); 
            ClassicAssert.IsTrue(result.Halls.Any());
        }

        [Test]
        public async Task GetScreeningForEditAsync_ShouldReturnNull_WhenIdIsInvalid()
        {
            // Act
            var result = await _screeningService.GetScreeningForEditAsync(999);

            // Assert
            ClassicAssert.IsNull(result);
        }

        [Test]
        public async Task UpdateScreeningAsync_ShouldUpdateHallAndDate()
        {
            // Arrange
            var screening = new Screening
            {
                MovieId = 1,
                CinemaHallId = 1, // Grand Hall
                StartDateTime = DateTime.Now
            };
            await _context.Screenings.AddAsync(screening);
            await _context.SaveChangesAsync();

            var updateModel = new ScreeningFormModel
            {
                Id = screening.Id,
                MovieId = 1,
                CinemaHallId = 2,
                StartDateTime = DateTime.Now.AddHours(5)
            };

            // Act
            await _screeningService.UpdateScreeningAsync(updateModel);

            // Assert
            var dbScreening = await _context.Screenings.FindAsync(screening.Id);
            ClassicAssert.AreEqual(2, dbScreening.CinemaHallId); // Трябва да е VIP Hall
        }

        [Test]
        public async Task DeleteScreeningAsync_ShouldRemoveScreening()
        {
            // Arrange
            var screening = new Screening { MovieId = 1, CinemaHallId = 1, StartDateTime = DateTime.Now };
            await _context.Screenings.AddAsync(screening);
            await _context.SaveChangesAsync();

            // Act
            await _screeningService.DeleteScreeningAsync(screening.Id);

            // Assert
            var count = await _context.Screenings.CountAsync();
            ClassicAssert.AreEqual(0, count);
        }
    }
}