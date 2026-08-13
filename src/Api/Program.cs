using Api.Middleware;
using Api.Services;
using Application;
using Application.Common.Interfaces;
using Infrastructure;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Resolved against the project directory so the database lands in db/ at the
// repository root regardless of the working directory the app is launched from.
var databasePath = Path.GetFullPath(
    builder.Configuration["Database:Path"] ?? Path.Combine("..", "..", "db", "app.db"),
    builder.Environment.ContentRootPath);
Directory.CreateDirectory(Path.GetDirectoryName(databasePath)!);

builder.Services.AddApplication();
builder.Services.AddInfrastructure($"Data Source={databasePath}");

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "IT03 Document Approval API",
        Version = "v1",
    });
});

const string SpaCorsPolicy = "spa";
builder.Services.AddCors(options =>
{
    options.AddPolicy(SpaCorsPolicy, policy => policy
        .WithOrigins("http://localhost:4200")
        .AllowAnyHeader()
        .AllowAnyMethod());
});

var app = builder.Build();

await app.Services.InitialiseDatabaseAsync();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors(SpaCorsPolicy);

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "IT03 Document Approval API v1");
    options.RoutePrefix = "swagger";
});

app.MapControllers();

app.Run();
