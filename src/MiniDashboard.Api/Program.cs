using MiniDashboard.Api.Repositories;
using MiniDashboard.Api.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MiniDashboard API", Version = "v1" });
});

// Dependency Injection
builder.Services.AddSingleton<IItemRepository, ItemRepository>();
builder.Services.AddScoped<IItemService, ItemService>();

var app = builder.Build();

// Middleware
app.UseHttpsRedirection();
app.UseAuthorization();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger(c => {
        c.PreSerializeFilters.Add((swagger, httpReq) => {
            swagger.Servers = new[] {
                new OpenApiServer { Url = $"https://{httpReq.Host.Value}" }
            };
        });
    });
    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MiniDashboard API v1");
        c.RoutePrefix = "swagger";
    });
}

app.MapControllers();
app.Run();

// Partial class to enable integration testing with WebApplicationFactory
public partial class Program { }
