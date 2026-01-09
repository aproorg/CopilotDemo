using CopilotDemo.Core.Entities;

namespace CopilotDemo.Tests;

/// <summary>
/// Base class for tests providing common test data
/// </summary>
public abstract class TestBase
{
  /// <summary>
  /// Gets a sample TodoItem for testing
  /// </summary>
  protected TodoItem SampleTodoItem => new TodoItem
  {
    Id = 1,
    Title = "Test Todo",
    Description = "Test Description",
    IsCompleted = false,
    CreatedAt = DateTime.UtcNow,
    CompletedAt = null
  };

  /// <summary>
  /// Gets a completed TodoItem for testing
  /// </summary>
  protected TodoItem CompletedTodoItem => new TodoItem
  {
    Id = 2,
    Title = "Completed Todo",
    Description = "Completed Description",
    IsCompleted = true,
    CreatedAt = DateTime.UtcNow.AddDays(-1),
    CompletedAt = DateTime.UtcNow
  };
}
