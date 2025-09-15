using Microsoft.AspNetCore.Mvc;
using todolist.dto;
using todolist.iservice;
using todolist.model;

namespace todolist.controller;

[ApiController]
[Route("api/todo")]
public class TodoController : ControllerBase
{
  private readonly ITodoService _todoService;

  public TodoController(ITodoService todoService)
  {
    _todoService = todoService;
  }

  [HttpGet]
  public async Task<ActionResult<IEnumerable<Todo>>> GetTodoList()
  {
    var todoList = await _todoService.GetAllAsync();
    return Ok(todoList);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<Todo>> GetTodo(int id)
  {
    var todo = await _todoService.GetTodoByIdAsync(id);
    return Ok(todo);
  }

  [HttpPost()]
  public async Task<ActionResult<Todo>> CreateTodo(TodoRequest request)
  {
    var todo = await _todoService.CreateAsync(request);
    return CreatedAtAction(nameof(GetTodo), new { id = todo.Id }, todo);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<Todo>> UpdateTodo(int id, TodoRequest request)
  {
    var todo = await _todoService.UpdateAsync(id, request);
    return todo is not null ? Ok(todo) : NotFound();
  }

  [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTodo(int id)
    {
        var success = await _todoService.DeleteAsync(id);
        return success ? NoContent() : NotFound();
    }
}