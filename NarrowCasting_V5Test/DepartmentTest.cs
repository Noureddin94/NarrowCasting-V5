using Microsoft.EntityFrameworkCore;
using Moq;
using NarrowCasting_V5.Data;
using NarrowCasting_V5.Interfaces;
using NarrowCasting_V5.Models;
using NarrowCasting_V5.Services;
using Xunit;

namespace NarrowCasting_V5Test;

public class DepartmentTest
{
    [Fact]
    public void TestDepartmentCreation()
    {
        // Arrange
        var department = new Department
        {
            Id = 1,
            Name = "IT",
            Description = "Information Technology Department"
        };
        // Act
        var departmentName = department.Name;
        var departmentDescription = department.Description;
        // Assert
        Assert.Equal("IT", departmentName);
        Assert.Equal("Information Technology Department", departmentDescription);
    }

    [Fact]
    public void TestDepartementUpdate()
    {
        // Arrange
        var department = new Department
        {
            Id = 1,
            Name = "IT",
            Description = "Information Technology Department"
        };
        // Act
        department.Name = "HR";
        department.Description = "Human Resources Department";
        // Assert
        Assert.Equal("HR", department.Name);
        Assert.Equal("Human Resources Department", department.Description);
    }

    [Fact]
    public void TestDepartementDelete()
    {
        // Arrange
        var department = new Department
        {
            Id = 1,
            Name = "IT",
            Description = "Information Technology Department"
        };
        // Act
        department = null;
        // Assert
        Assert.Null(department);
    }

    // Using Service layer to test the CRUD operations for Department entity
    [Fact]
    public async Task CreateDepartment_ShouldSaveDepartment()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("DepartmentDb")
            .Options;

        var context = new ApplicationDbContext(options);

        var auditMock = new Mock<IAuditService>();

        var service = new DepartmentService(context, auditMock.Object);

        var department = new Department
        {
            Name = "IT",
            Description = "Information Technology"
        };

        // Act
        await service.CreateAsync(department, "user1");

        // Assert
        var savedDepartment = await context.Departments.FirstOrDefaultAsync();

        Assert.NotNull(savedDepartment);
        Assert.Equal("IT", savedDepartment.Name);

        auditMock.Verify(a =>
            a.LogAsync("Department", department.Id, "Create", "user1", null),
            Times.Once);
    }
}
