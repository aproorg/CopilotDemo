using CopilotDemo.Core.Entities;
using CopilotDemo.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CopilotDemo.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodoController : ControllerBase
{
    private readonly ITodoService _todoService;
    private readonly ILogger<TodoController> _logger;

    public TodoController(ITodoService todoService, ILogger<TodoController> logger)
    {
        _todoService = todoService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoItem>>> GetAll()
    {
        var todos = await _todoService.GetAllTodosAsync();
        return Ok(todos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TodoItem>> GetById(int id)
    {
        var todo = await _todoService.GetTodoByIdAsync(id);
        if (todo == null)
        {
            return NotFound();
        }
        return Ok(todo);
    }

    [HttpPost]
    public async Task<ActionResult<TodoItem>> Create([FromBody] CreateTodoRequest request)
    {
        var todo = await _todoService.CreateTodoAsync(request.Title, request.Description);
        return CreatedAtAction(nameof(GetById), new { id = todo.Id }, todo);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TodoItem>> Update(int id, [FromBody] UpdateTodoRequest request)
    {
        var todo = await _todoService.UpdateTodoAsync(id, request.Title, request.Description, request.IsCompleted);
        if (todo == null)
        {
            return NotFound();
        }
        return Ok(todo);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _todoService.DeleteTodoAsync(id);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpPost("{id}/complete")]
    public async Task<ActionResult<TodoItem>> MarkAsCompleted(int id)
    {
        var todo = await _todoService.MarkAsCompletedAsync(id);
        if (todo == null)
        {
            return NotFound();
        }
        return Ok(todo);
    }
}

public record CreateTodoRequest(string Title, string? Description);
public record UpdateTodoRequest(string Title, string? Description, bool IsCompleted);
