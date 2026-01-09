using CopilotDemo.Core.Entities;
using CopilotDemo.Core.Interfaces;
using CopilotDemo.Core.Services;
using Moq;

namespace CopilotDemo.Tests;

/// <summary>
/// TodoService tests
/// </summary>
public class TodoServiceTests : TestBase
{
  private readonly Mock<ITodoRepository> todoRepositoryMock;
  private readonly TodoService service;

  /// <summary>
  /// Initializes a new instance of the <see cref="TodoServiceTests"/> class.
  /// </summary>
  public TodoServiceTests()
  {
    todoRepositoryMock = new Mock<ITodoRepository>();
    service = new TodoService(todoRepositoryMock.Object);
  }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented

  [Fact]
  public async Task GetTodoByIdAsync_WithValidId_ReturnsTodoItem()
  {
    // Arrange
    var expectedTodo = SampleTodoItem;
    todoRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(expectedTodo);

    // Act
    var result = await service.GetTodoByIdAsync(1);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(expectedTodo.Id, result.Id);
    Assert.Equal(expectedTodo.Title, result.Title);
    Assert.Equal(expectedTodo.Description, result.Description);
    Assert.Equal(expectedTodo.IsCompleted, result.IsCompleted);

    todoRepositoryMock.Verify(x => x.GetByIdAsync(1), Times.Once);
  }

  [Fact]
  public async Task GetTodoByIdAsync_WithInvalidId_ReturnsNull()
  {
    // Arrange
    todoRepositoryMock.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((TodoItem?)null);

    // Act
    var result = await service.GetTodoByIdAsync(999);

    // Assert
    Assert.Null(result);

    todoRepositoryMock.Verify(x => x.GetByIdAsync(999), Times.Once);
  }

  [Fact]
  public async Task GetAllTodosAsync_WithExistingTodos_ReturnsList()
  {
    // Arrange
    var expectedTodos = new List<TodoItem>
    {
      SampleTodoItem,
      CompletedTodoItem
    };

    todoRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(expectedTodos);

    // Act
    var result = await service.GetAllTodosAsync();

    // Assert
    Assert.NotNull(result);
    var todoList = result.ToList();
    Assert.Equal(2, todoList.Count);
    Assert.Equal("Test Todo", todoList[0].Title);
    Assert.Equal("Completed Todo", todoList[1].Title);

    todoRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
  }

  [Fact]
  public async Task GetAllTodosAsync_WithNoTodos_ReturnsEmptyList()
  {
    // Arrange
    todoRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<TodoItem>());

    // Act
    var result = await service.GetAllTodosAsync();

    // Assert
    Assert.NotNull(result);
    Assert.Empty(result);

    todoRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
  }

  [Fact]
  public async Task CreateTodoAsync_WithValidData_CreatesAndReturnsTodoItem()
  {
    // Arrange
    var title = "New Todo";
    var description = "New Description";
    TodoItem? capturedTodo = null;

    todoRepositoryMock.Setup(x => x.AddAsync(It.IsAny<TodoItem>()))
                     .Callback<TodoItem>(todo => capturedTodo = todo)
                     .ReturnsAsync((TodoItem todo) => todo);

    // Act
    var result = await service.CreateTodoAsync(title, description);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(title, result.Title);
    Assert.Equal(description, result.Description);
    Assert.False(result.IsCompleted);
    Assert.Null(result.CompletedAt);
    Assert.True((DateTime.UtcNow - result.CreatedAt).TotalSeconds < 1);

    Assert.NotNull(capturedTodo);
    Assert.Equal(title, capturedTodo.Title);
    Assert.Equal(description, capturedTodo.Description);

    todoRepositoryMock.Verify(x => x.AddAsync(It.IsAny<TodoItem>()), Times.Once);
  }

  [Fact]
  public async Task CreateTodoAsync_WithNullDescription_CreatesWithNullDescription()
  {
    // Arrange
    var title = "New Todo";

    todoRepositoryMock.Setup(x => x.AddAsync(It.IsAny<TodoItem>()))
                     .ReturnsAsync((TodoItem todo) => todo);

    // Act
    var result = await service.CreateTodoAsync(title, null);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(title, result.Title);
    Assert.Null(result.Description);
    Assert.False(result.IsCompleted);

    todoRepositoryMock.Verify(x => x.AddAsync(It.Is<TodoItem>(t => t.Description == null)), Times.Once);
  }

  [Fact]
  public async Task UpdateTodoAsync_WithValidIdAndData_UpdatesAndReturnsTodoItem()
  {
    // Arrange
    var existingTodo = new TodoItem
    {
      Id = 1,
      Title = "Old Title",
      Description = "Old Description",
      IsCompleted = false,
      CreatedAt = DateTime.UtcNow.AddDays(-1),
      CompletedAt = null
    };

    var updatedTitle = "Updated Title";
    var updatedDescription = "Updated Description";

    todoRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(existingTodo);
    todoRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<TodoItem>()))
                     .ReturnsAsync((TodoItem todo) => todo);

    // Act
    var result = await service.UpdateTodoAsync(1, updatedTitle, updatedDescription, false);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(updatedTitle, result.Title);
    Assert.Equal(updatedDescription, result.Description);
    Assert.False(result.IsCompleted);
    Assert.Null(result.CompletedAt);

    todoRepositoryMock.Verify(x => x.GetByIdAsync(1), Times.Once);
    todoRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<TodoItem>()), Times.Once);
  }

  [Fact]
  public async Task UpdateTodoAsync_MarkingAsCompleted_SetsCompletedAt()
  {
    // Arrange
    var existingTodo = new TodoItem
    {
      Id = 1,
      Title = "Test Todo",
      Description = "Test Description",
      IsCompleted = false,
      CreatedAt = DateTime.UtcNow.AddDays(-1),
      CompletedAt = null
    };

    todoRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(existingTodo);
    todoRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<TodoItem>()))
                     .ReturnsAsync((TodoItem todo) => todo);

    // Act
    var result = await service.UpdateTodoAsync(1, "Test Todo", "Test Description", true);

    // Assert
    Assert.NotNull(result);
    Assert.True(result.IsCompleted);
    Assert.NotNull(result.CompletedAt);
    Assert.True((DateTime.UtcNow - result.CompletedAt.Value).TotalSeconds < 1);

    todoRepositoryMock.Verify(x => x.UpdateAsync(It.Is<TodoItem>(t => 
      t.IsCompleted && t.CompletedAt != null)), Times.Once);
  }

  [Fact]
  public async Task UpdateTodoAsync_UnmarkingAsCompleted_ClearsCompletedAt()
  {
    // Arrange
    var existingTodo = new TodoItem
    {
      Id = 1,
      Title = "Test Todo",
      Description = "Test Description",
      IsCompleted = true,
      CreatedAt = DateTime.UtcNow.AddDays(-1),
      CompletedAt = DateTime.UtcNow
    };

    todoRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(existingTodo);
    todoRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<TodoItem>()))
                     .ReturnsAsync((TodoItem todo) => todo);

    // Act
    var result = await service.UpdateTodoAsync(1, "Test Todo", "Test Description", false);

    // Assert
    Assert.NotNull(result);
    Assert.False(result.IsCompleted);
    Assert.Null(result.CompletedAt);

    todoRepositoryMock.Verify(x => x.UpdateAsync(It.Is<TodoItem>(t => 
      !t.IsCompleted && t.CompletedAt == null)), Times.Once);
  }

  [Fact]
  public async Task UpdateTodoAsync_WithInvalidId_ReturnsNull()
  {
    // Arrange
    todoRepositoryMock.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((TodoItem?)null);

    // Act
    var result = await service.UpdateTodoAsync(999, "Title", "Description", false);

    // Assert
    Assert.Null(result);

    todoRepositoryMock.Verify(x => x.GetByIdAsync(999), Times.Once);
    todoRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<TodoItem>()), Times.Never);
  }

  [Fact]
  public async Task DeleteTodoAsync_WithValidId_ReturnsTrue()
  {
    // Arrange
    todoRepositoryMock.Setup(x => x.DeleteAsync(1)).ReturnsAsync(true);

    // Act
    var result = await service.DeleteTodoAsync(1);

    // Assert
    Assert.True(result);

    todoRepositoryMock.Verify(x => x.DeleteAsync(1), Times.Once);
  }

  [Fact]
  public async Task DeleteTodoAsync_WithInvalidId_ReturnsFalse()
  {
    // Arrange
    todoRepositoryMock.Setup(x => x.DeleteAsync(999)).ReturnsAsync(false);

    // Act
    var result = await service.DeleteTodoAsync(999);

    // Assert
    Assert.False(result);

    todoRepositoryMock.Verify(x => x.DeleteAsync(999), Times.Once);
  }

  [Fact]
  public async Task MarkAsCompletedAsync_WithValidId_MarksAsCompletedAndReturnsTodoItem()
  {
    // Arrange
    var existingTodo = new TodoItem
    {
      Id = 1,
      Title = "Test Todo",
      Description = "Test Description",
      IsCompleted = false,
      CreatedAt = DateTime.UtcNow.AddDays(-1),
      CompletedAt = null
    };

    todoRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(existingTodo);
    todoRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<TodoItem>()))
                     .ReturnsAsync((TodoItem todo) => todo);

    // Act
    var result = await service.MarkAsCompletedAsync(1);

    // Assert
    Assert.NotNull(result);
    Assert.True(result.IsCompleted);
    Assert.NotNull(result.CompletedAt);
    Assert.True((DateTime.UtcNow - result.CompletedAt.Value).TotalSeconds < 1);

    todoRepositoryMock.Verify(x => x.GetByIdAsync(1), Times.Once);
    todoRepositoryMock.Verify(x => x.UpdateAsync(It.Is<TodoItem>(t => 
      t.IsCompleted && t.CompletedAt != null)), Times.Once);
  }

  [Fact]
  public async Task MarkAsCompletedAsync_WithInvalidId_ReturnsNull()
  {
    // Arrange
    todoRepositoryMock.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((TodoItem?)null);

    // Act
    var result = await service.MarkAsCompletedAsync(999);

    // Assert
    Assert.Null(result);

    todoRepositoryMock.Verify(x => x.GetByIdAsync(999), Times.Once);
    todoRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<TodoItem>()), Times.Never);
  }

  [Fact]
  public async Task MarkAsCompletedAsync_AlreadyCompleted_UpdatesCompletedAt()
  {
    // Arrange
    var oldCompletedAt = DateTime.UtcNow.AddDays(-1);
    var existingTodo = new TodoItem
    {
      Id = 1,
      Title = "Test Todo",
      Description = "Test Description",
      IsCompleted = true,
      CreatedAt = DateTime.UtcNow.AddDays(-2),
      CompletedAt = oldCompletedAt
    };

    todoRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(existingTodo);
    todoRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<TodoItem>()))
                     .ReturnsAsync((TodoItem todo) => todo);

    // Act
    var result = await service.MarkAsCompletedAsync(1);

    // Assert
    Assert.NotNull(result);
    Assert.True(result.IsCompleted);
    Assert.NotNull(result.CompletedAt);
    Assert.NotEqual(oldCompletedAt, result.CompletedAt);
    Assert.True((DateTime.UtcNow - result.CompletedAt.Value).TotalSeconds < 1);

    todoRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<TodoItem>()), Times.Once);
  }

  [Fact]
  public async Task GetAllTodosAsync_RepositoryThrowsException_ExceptionPropagates()
  {
    // Arrange
    var expectedException = new Exception("Database Error");
    todoRepositoryMock.Setup(x => x.GetAllAsync()).ThrowsAsync(expectedException);

    // Act & Assert
    var exception = await Assert.ThrowsAsync<Exception>(() => service.GetAllTodosAsync());
    Assert.Equal("Database Error", exception.Message);

    todoRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
  }

  [Fact]
  public async Task CreateTodoAsync_RepositoryThrowsException_ExceptionPropagates()
  {
    // Arrange
    var expectedException = new Exception("Database Error");
    todoRepositoryMock.Setup(x => x.AddAsync(It.IsAny<TodoItem>())).ThrowsAsync(expectedException);

    // Act & Assert
    var exception = await Assert.ThrowsAsync<Exception>(() => 
      service.CreateTodoAsync("Test", "Description"));
    Assert.Equal("Database Error", exception.Message);

    todoRepositoryMock.Verify(x => x.AddAsync(It.IsAny<TodoItem>()), Times.Once);
  }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning restore SA1600 // Elements should be documented
}
