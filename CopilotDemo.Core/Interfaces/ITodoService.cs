using CopilotDemo.Core.Entities;

namespace CopilotDemo.Core.Interfaces;

public interface ITodoService
{
    Task<TodoItem?> GetTodoByIdAsync(int id);
    Task<IEnumerable<TodoItem>> GetAllTodosAsync();
    Task<TodoItem> CreateTodoAsync(string title, string? description);
    Task<TodoItem?> UpdateTodoAsync(int id, string title, string? description, bool isCompleted);
    Task<bool> DeleteTodoAsync(int id);
    Task<TodoItem?> MarkAsCompletedAsync(int id);
}
