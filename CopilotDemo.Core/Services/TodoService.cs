using CopilotDemo.Core.Entities;
using CopilotDemo.Core.Interfaces;

namespace CopilotDemo.Core.Services;

public class TodoService : ITodoService
{
    private readonly ITodoRepository _todoRepository;

    public TodoService(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task<TodoItem?> GetTodoByIdAsync(int id)
    {
        return await _todoRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<TodoItem>> GetAllTodosAsync()
    {
        return await _todoRepository.GetAllAsync();
    }

    public async Task<TodoItem> CreateTodoAsync(string title, string? description)
    {
        var todoItem = new TodoItem
        {
            Title = title,
            Description = description,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };

        return await _todoRepository.AddAsync(todoItem);
    }

    public async Task<TodoItem?> UpdateTodoAsync(int id, string title, string? description, bool isCompleted)
    {
        var existingTodo = await _todoRepository.GetByIdAsync(id);
        if (existingTodo == null)
        {
            return null;
        }

        existingTodo.Title = title;
        existingTodo.Description = description;
        existingTodo.IsCompleted = isCompleted;

        if (isCompleted && existingTodo.CompletedAt == null)
        {
            existingTodo.CompletedAt = DateTime.UtcNow;
        }
        else if (!isCompleted)
        {
            existingTodo.CompletedAt = null;
        }

        return await _todoRepository.UpdateAsync(existingTodo);
    }

    public async Task<bool> DeleteTodoAsync(int id)
    {
        return await _todoRepository.DeleteAsync(id);
    }

    public async Task<TodoItem?> MarkAsCompletedAsync(int id)
    {
        var existingTodo = await _todoRepository.GetByIdAsync(id);
        if (existingTodo == null)
        {
            return null;
        }

        existingTodo.IsCompleted = true;
        existingTodo.CompletedAt = DateTime.UtcNow;

        return await _todoRepository.UpdateAsync(existingTodo);
    }
}
