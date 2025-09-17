using DotNetEnv;
using extensions;


Env.Load();

// สร้างแอป
var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
// builder.Services.AddAuthorization();

//JWT Configuration
builder.Configuration["Jwt:SecretKey"] = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? 
    throw new InvalidOperationException("JWT_SECRET_KEY not found");

// Database Configuration
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") ?? 
    throw new InvalidOperationException("DATABASE_URL not found in environment variables");

// Extensions
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddDatabaseServices(connectionString);
builder.Services.AddApplicationServices();
builder.Services.AddCorsPolicy();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.UseRouting();

// Map controllers
app.MapControllers();

// Custom endpoints
app.MapGet("/hello/", () => "hello world 😺");
app.MapGet("/hello/{name}", (string name) => $"{name} ❤️");

app.Run();