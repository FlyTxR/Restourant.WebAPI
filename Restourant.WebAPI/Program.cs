using Business.Services;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Diagnostics;
using Business.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Configure DbContext
builder.Services.AddDbContext<SqlContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Singleton Settings ← AGGIUNGI QUESTA RIGA
builder.Services.AddSingleton<RestaurantSettings>();

// Register application services
builder.Services.AddScoped<MenuItemService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<BookingService>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Restaurant API",
        Version = "v1",
        Description = "API for restaurant menu management"
    });
});

var app = builder.Build();

// Load settings from database on startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SqlContext>();
    var settings = scope.ServiceProvider.GetRequiredService<RestaurantSettings>();

    await settings.LoadFromDatabaseAsync(context);
    Console.WriteLine("✅ Configurazione caricata dal database");
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Global exception handler
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
        if (exceptionFeature != null)
        {
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError(exceptionFeature.Error,
                "Unhandled exception on {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            await context.Response.WriteAsJsonAsync(new
            {
                error = "An error occurred processing your request",
                path = context.Request.Path.ToString()
            });
        }
    });
});

app.UseHttpsRedirection();

app.UseCors("AllowAngularApp");

app.UseAuthorization();

app.MapControllers();

app.Run();
