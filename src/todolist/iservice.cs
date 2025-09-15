using todolist.dto;
using todolist.model;

namespace todolist.iservice;

public interface ITodoService
{
  Task<IEnumerable<Todo>> GetAllAsync();
  Task<Todo?> GetTodoByIdAsync(int id);
  Task<Todo> CreateAsync(TodoRequest request);
  Task<Todo?> UpdateAsync(int id, TodoRequest request);
  Task<bool> DeleteAsync(int id);
}