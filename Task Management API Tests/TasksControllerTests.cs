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

            mockTaskService.Verify(
    service => service.GetById(
        1,
        It.IsAny<CancellationToken>()),
    Times.Once);
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
            mockTaskService.Verify(
    service => service.GetById(
        999,
        It.IsAny<CancellationToken>()),
    Times.Once);
        }

        [Fact]
        public async Task GetAll_WhenTasksExist_ReturnsOkWithTasks()
        {
            // Arrange
            var expectedTasks = new List<TaskItem>
    {
        new TaskItem
        {
            Id = 1,
            Title = "Learn unit testing",
            IsCompleted = false,
            Description = "Practice xUnit and Moq"
        },
        new TaskItem
        {
            Id = 2,
            Title = "Learn Docker",
            IsCompleted = true,
            Description = "Practice Docker Compose"
        }
    };

            var mockTaskService = new Mock<ITaskService>();

            mockTaskService
                .Setup(service => service.GetAll(
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTasks);

            var controller = new TasksController(mockTaskService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            // Act
            var result = await controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualTasks = Assert.IsType<List<TaskItem>>(okResult.Value);

            Assert.Equal(2, actualTasks.Count);
            Assert.Equal(expectedTasks[0].Title, actualTasks[0].Title);
            Assert.Equal(expectedTasks[1].Title, actualTasks[1].Title);

            mockTaskService.Verify(
    service => service.GetAll(
        It.IsAny<CancellationToken>()),
    Times.Once);
        }


        [Fact]
        public async Task GetAll_WhenNoTasksExist_ReturnsOkWithEmptyList()
        {
            // Arrange
            var mockTaskService = new Mock<ITaskService>();

            mockTaskService
                .Setup(service => service.GetAll(
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TaskItem>());

            var controller = new TasksController(mockTaskService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            // Act
            var result = await controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualTasks = Assert.IsType<List<TaskItem>>(okResult.Value);

            Assert.Empty(actualTasks);
            mockTaskService.Verify(
    service => service.GetAll(
        It.IsAny<CancellationToken>()),
    Times.Once);
        }

        [Fact]
        public async Task Create_WhenTaskIsCreated_ReturnsCreatedAtAction()
        {
            // Arrange
            var inputTask = new TaskItem
            {
                Title = "Learn unit testing",
                IsCompleted = false,
                Description = "Practice xUnit and Moq"
            };

            var createdTask = new TaskItem
            {
                Id = 10,
                Title = inputTask.Title,
                IsCompleted = inputTask.IsCompleted,
                Description = inputTask.Description
            };

            var mockTaskService = new Mock<ITaskService>();

            mockTaskService
                .Setup(service => service.Create(
                    inputTask,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdTask);

            var controller = new TasksController(mockTaskService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            // Act
            var result = await controller.Create(inputTask);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);

            Assert.Equal(nameof(TasksController.GetById), createdResult.ActionName);

            Assert.NotNull(createdResult.RouteValues);
            Assert.Equal(10, createdResult.RouteValues["id"]);

            var actualTask = Assert.IsType<TaskItem>(createdResult.Value);
            Assert.Equal(10, actualTask.Id);
            Assert.Equal(inputTask.Title, actualTask.Title);
            Assert.Equal(inputTask.IsCompleted, actualTask.IsCompleted);
            Assert.Equal(inputTask.Description, actualTask.Description);

            mockTaskService.Verify(
                service => service.Create(
                    inputTask,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Update_WhenTaskExists_ReturnsNoContent()
        {
            // Arrange
            var task = new TaskItem
            {
                Id = 1,
                Title = "Updated task",
                IsCompleted = true,
                Description = "Updated description"
            };

            var mockTaskService = new Mock<ITaskService>();

            mockTaskService
                .Setup(service => service.Update(
                    1,
                    task,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var controller = new TasksController(mockTaskService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            // Act
            var result = await controller.Update(1, task);

            // Assert
            Assert.IsType<NoContentResult>(result);

            mockTaskService.Verify(
                service => service.Update(
                    1,
                    task,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Update_WhenTaskDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var task = new TaskItem
            {
                Id = 999,
                Title = "Updated task",
                IsCompleted = true,
                Description = "Updated description"
            };

            var mockTaskService = new Mock<ITaskService>();

            mockTaskService
                .Setup(service => service.Update(
                    999,
                    task,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var controller = new TasksController(mockTaskService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            // Act
            var result = await controller.Update(999, task);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            mockTaskService.Verify(
    service => service.Update(
        999,
        task,
        It.IsAny<CancellationToken>()),
    Times.Once);

        }
        [Fact]
        public async Task Delete_WhenTaskExists_ReturnsNoContent()
        {
            // Arrange
            var mockTaskService = new Mock<ITaskService>();

            mockTaskService
                .Setup(service => service.Delete(
                    1,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var controller = new TasksController(mockTaskService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            // Act
            var result = await controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(result);

            mockTaskService.Verify(
                service => service.Delete(
                    1,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Delete_WhenTaskDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var mockTaskService = new Mock<ITaskService>();

            mockTaskService
                .Setup(service => service.Delete(
                    999,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var controller = new TasksController(mockTaskService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            // Act
            var result = await controller.Delete(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            mockTaskService.Verify(
    service => service.Delete(
        999,
        It.IsAny<CancellationToken>()),
    Times.Once);
        }

    }
}