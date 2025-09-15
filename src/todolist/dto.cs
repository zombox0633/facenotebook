namespace todolist.dto;

public record TodoRequest(string Title, bool IsCompleted);

public record TodoResponse(int Id, string Title, bool IsCompleted);
