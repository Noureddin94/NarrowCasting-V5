using Microsoft.EntityFrameworkCore;
using Moq;
using NarrowCasting_V5.Data;
using NarrowCasting_V5.Interfaces;
using NarrowCasting_V5.Models;
using NarrowCasting_V5.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// =========================================
// TS01 – FR-SL4
// GetActiveScreens_ReturnsOnlyActiveScreens
// =========================================
namespace NarrowCasting_V5Test.ScreensTest
{
    public class ScreenServiceTests
    {
        // Because we can't moq the ApplicationDbContext directly
        // We need to use InMemory database
        private ApplicationDbContext CreateInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task GetActiveScreens_ReturnsOnlyActiveScreens()
        {
            // Arrange
            var db = CreateInMemoryDb();

            db.Departments.Add(new Department { Id = 1, Name = "Algemeen" });
            await db.SaveChangesAsync();

            db.Screens.AddRange
            (
                new Screen { Id = 1, Name = "WachtKamer A", Location = "Begane Grond", IsActive = true, DepartmentId = 1 },
                new Screen { Id = 2, Name = "Radiologie", Location = "Begane Grond", IsActive = true, DepartmentId = 1 },
                new Screen { Id = 3, Name = "Apotheek", Location = "Begane Grond", IsActive = false, DepartmentId = 1 }
            );
            await db.SaveChangesAsync();

            var mockAudit = new Mock<IAuditService>();
            var service = new ScreenService(db, mockAudit.Object);

            // Act
            var result = await service.GetActiveAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, s => Assert.True(s.IsActive));
        }
    }
}
