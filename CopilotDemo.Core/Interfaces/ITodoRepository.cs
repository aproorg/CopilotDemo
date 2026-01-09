using CopilotDemo.Core.Entities;

namespace CopilotDemo.Core.Interfaces;

public interface ITodoRepository
{
    Task<TodoItem?> GetByIdAsync(int id);
    Task<IEnumerable<TodoItem>> GetAllAsync();
    Task<TodoItem> AddAsync(TodoItem todoItem);
    Task<TodoItem?> UpdateAsync(TodoItem todoItem);
    Task<bool> DeleteAsync(int id);
}
