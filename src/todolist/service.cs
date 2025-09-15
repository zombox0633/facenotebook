using todolist.dto;
using todolist.iservice;
using todolist.model;

namespace todolist.service;

public class TodoService : ITodoService
{
  private readonly List<Todo> _todos = new()
  {
    new(1, "😺", true),
    new(2, "🦄", false),
  };

  public Task<IEnumerable<Todo>> GetAllAsync()
  {
    return Task.FromResult(_todos.AsEnumerable());
  }

  public Task<Todo?> GetTodoByIdAsync(int id)
  {
    var todo = _todos.FirstOrDefault(t => t.Id == id);
    return Task.FromResult(todo);
  }

  public Task<Todo> CreateAsync(TodoRequest request)
  {
    var IsCompleted = request.IsCompleted ? request.IsCompleted : false;
    var newTodo = new Todo(_todos.Count + 1, request.Title, IsCompleted);
    _todos.Add(newTodo);
    return Task.FromResult(newTodo);
  }

  public Task<Todo?> UpdateAsync(int Id, TodoRequest request)
  {
    var todo = _todos.FirstOrDefault(t => t.Id == Id);
    if (todo is null) return Task.FromResult<Todo?>(null);

    var updatedTodo = todo with { Title = request.Title, IsCompleted = request.IsCompleted };
    _todos[_todos.FindIndex(t => t.Id == Id)] = updatedTodo;
    return Task.FromResult<Todo?>(updatedTodo);
  }

  public Task<bool> DeleteAsync(int id)
    {
        var todo = _todos.FirstOrDefault(t => t.Id == id);
        if (todo is null) return Task.FromResult(false);

        _todos.Remove(todo);
        return Task.FromResult(true);
    }
}