using ArtPort.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("secrets.json", optional: false, reloadOnChange: true);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseNpgsql(connectionString)
    );

builder.Services.AddOpenApi();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Check database connection
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        var canConnect = db.Database.CanConnect();
        Console.WriteLine(canConnect ? "Database Connection successful." :
        "[WARNING] Database connection failed.");
    }
    catch (Exception ex)
    {
        Console.WriteLine("[ERROR] Database Connection Failed During Startup.");
        Console.WriteLine(ex.ToString());
        throw;
    }
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
