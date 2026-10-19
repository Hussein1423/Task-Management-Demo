using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Task_Management_API.Controllers;
using Task_Management_API.Entities;
using Task_Management_API.Services;

namespace Task_Management_API_Tests
{
    public class TasksControllerTests
    {
        [Fact]
        public async Task GetById_WhenTaskExists_ReturnsOkResult()
        {
            // Arrange
            var expectedTask = new TaskItem
            {
                Id = 1,
                Title = "Learn unit testing",
                IsCompleted = false,
                Description = "Practice xUnit and Moq"
            };

            var mockTaskService = new Mock<ITaskService>();

            mockTaskService
                .Setup(service => service.GetById(
                    1,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTask);

            var controller = new TasksController(mockTaskService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            // Act
            var result = await controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);

            var actualTask = Assert.IsType<TaskItem>(okResult.Value);

            Assert.Equal(expectedTask.Id, actualTask.Id);
            Assert.Equal(expectedTask.Title, actualTask.Title);
            Assert.Equal(expectedTask.IsCompleted, actualTask.IsCompleted);
            Assert.Equal(expectedTask.Description, actualTask.Description);
        }


        [Fact]
        public async Task GetById_WhenTaskDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var mockTaskService = new Mock<ITaskService>();

            mockTaskService
                .Setup(service => service.GetById(
                    999,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((TaskItem?)null);

            var controller = new TasksController(mockTaskService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            // Act
            var result = await controller.GetById(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

    }
}