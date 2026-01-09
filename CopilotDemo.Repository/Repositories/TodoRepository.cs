using CopilotDemo.Core.Entities;
using CopilotDemo.Core.Interfaces;
using CopilotDemo.Repository.Data;
using Microsoft.EntityFrameworkCore;

namespace CopilotDemo.Repository.Repositories;

public class TodoRepository : ITodoRepository
{
  private readonly ApplicationDbContext _context;

  public TodoRepository(ApplicationDbContext context)
  {
    _context = context;
  }

  public async Task<TodoItem?> GetByIdAsync(int id)
  {
    return await _context.TodoItems.FindAsync(id);
  }

  public async Task<IEnumerable<TodoItem>> GetAllAsync()
  {
    return await _context.TodoItems.ToListAsync();
  }

  public async Task<TodoItem> AddAsync(TodoItem todoItem)
  {
    _context.TodoItems.Add(todoItem);
    await _context.SaveChangesAsync();
    return todoItem;
  }

  public async Task<TodoItem?> UpdateAsync(TodoItem todoItem)
  {
    var existingTodo = await _context.TodoItems.FindAsync(todoItem.Id);
    if (existingTodo == null)
    {
      return null;
    }

    _context.Entry(existingTodo).CurrentValues.SetValues(todoItem);
    await _context.SaveChangesAsync();
    return existingTodo;
  }

  public async Task<bool> DeleteAsync(int id)
  {
    var todoItem = await _context.TodoItems.FindAsync(id);
    if (todoItem == null)
    {
      return false;
    }

    _context.TodoItems.Remove(todoItem);
    await _context.SaveChangesAsync();
    return true;
  }
}
