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

namespace NarrowCasting_V5Test
{
    public class PlaylistServiceTests
    {
        public const int MaxItems = 10;
        private ApplicationDbContext CreateInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task AddItem_ExceedsMaxItems_ThrowsException()
        {
            // Arrange
            var db = CreateInMemoryDb();

            // Create required dependencies
            var department = new Department { Id = 1, Name = "Test Dept" };
            db.Departments.Add(department);
            await db.SaveChangesAsync();

            var screen = new Screen { Id = 1, Name = "Test Screen", Location = "Test", IsActive = true, DepartmentId = 1 };
            db.Screens.Add(screen);
            await db.SaveChangesAsync();

            var playlist = new Playlist
            {
                Name = "Test Playlist",
                CreatedAt = DateTime.UtcNow,
                ScreenId = 1,
                CreatedById = "user-1"
            };
            db.Playlists.Add(playlist);
            await db.SaveChangesAsync();

            var mediaFile = new MediaFile
            {
                FileName = "test.mp4",
                FilePath = "/files/test.mp4",
                UploadedAt = DateTime.UtcNow,
                UploadedById = "user-1"
            };
            db.MediaFiles.Add(mediaFile);
            await db.SaveChangesAsync();

            var mockAudit = new Mock<IAuditService>();
            var service = new PlaylistService(db, mockAudit.Object, maxItems: 10);

            string userId = "user-1";

            // Act: Add exactly MaxItems items
            for (int i = 0; i < MaxItems; i++)
            {
                var item = new PlaylistItem
                {
                    Order = i,
                    DurationSeconds = 10,
                    MediaFileId = mediaFile.Id
                };
                await service.AddItemAsync(playlist.Id, item, userId);
            }

            // Try to add one more – should throw
            var extraItem = new PlaylistItem
            {
                Order = MaxItems,
                DurationSeconds = 10,
                MediaFileId = mediaFile.Id
            };

            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.AddItemAsync(playlist.Id, extraItem, userId));
        }
    }
}