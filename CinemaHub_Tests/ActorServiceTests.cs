using CinemaHub.Data;
using CinemaHub.Data.Models;
using CinemaHub.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using NUnit.Framework.Legacy; // <--- ВАЖНО
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaHub.Tests
{
    [TestFixture]
    public class ActorServiceTests
    {
        private ApplicationDbContext _context;
        private IActorService _actorService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "ActorDb_" + Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _actorService = new ActorService(_context);
        }

        [TearDown]
        public void Teardown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task AddActorAsync_ShouldAddActor()
        {
            // Arrange
            var actor = new Actor { Name = "Leo", Bio = "Oscar winner" };

            // Act
            await _actorService.AddActorAsync(actor);

            // Assert
            var count = await _context.Actors.CountAsync();
            ClassicAssert.AreEqual(1, count);

            var savedActor = await _context.Actors.FirstAsync();
            ClassicAssert.AreEqual("Leo", savedActor.Name);
        }

        [Test]
        public async Task GetAllActorsAsync_ShouldReturnList()
        {
            // Arrange
            _context.Actors.Add(new Actor { Name = "A1", Bio = "B1" });
            _context.Actors.Add(new Actor { Name = "A2", Bio = "B2" });
            await _context.SaveChangesAsync();

            // Act
            var result = await _actorService.GetAllActorsAsync();

            // Assert
            ClassicAssert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetActorByIdAsync_ShouldReturnActor()
        {
            // Arrange
            var actor = new Actor { Name = "Target", Bio = "B" };
            await _context.Actors.AddAsync(actor);
            await _context.SaveChangesAsync();

            // Act
            var result = await _actorService.GetActorByIdAsync(actor.Id);

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual("Target", result.Name);
        }

        [Test]
        public async Task UpdateActorAsync_ShouldUpdateFields()
        {
            // Arrange
            var actor = new Actor { Name = "Old Name", Bio = "Old Bio" };
            await _context.Actors.AddAsync(actor);
            await _context.SaveChangesAsync();

            // Act
            actor.Name = "New Name";
            actor.Bio = "New Bio";
            await _actorService.UpdateActorAsync(actor);

            // Assert
            var dbActor = await _context.Actors.FindAsync(actor.Id);
            ClassicAssert.AreEqual("New Name", dbActor.Name);
            ClassicAssert.AreEqual("New Bio", dbActor.Bio);
        }

        [Test]
        public async Task DeleteActorAsync_ShouldRemoveActor()
        {
            // Arrange
            var actor = new Actor { Name = "Delete Me", Bio = "..." };
            await _context.Actors.AddAsync(actor);
            await _context.SaveChangesAsync();

            // Act
            await _actorService.DeleteActorAsync(actor.Id);

            // Assert
            var count = await _context.Actors.CountAsync();
            ClassicAssert.AreEqual(0, count);
        }

        [Test]
        public async Task GetActorDetailsAsync_ShouldReturnNull_IfIdNotFound()
        {
            // Act
            var result = await _actorService.GetActorDetailsAsync(9999);

            // Assert
            ClassicAssert.IsNull(result);
        }
    }
}