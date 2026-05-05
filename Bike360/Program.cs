using Bike360.Application.Middleware;
using Bike360.Infrastructure;
using Bike360.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Infrastructure
builder.Services.AddInfrastructure(builder.Configuration);


//CORS 
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",policy =>
    {
       if(builder.Environment.IsDevelopment())
       {
            //permission for development environment (allow localhost and frontend dev server)
            policy.WithOrigins(allowedOrigins)
                 .AllowAnyMethod()
                 .AllowAnyHeader()
                 .AllowCredentials();
       }
       else
       {
            //strict permission for production environment (only allow frontend domain)
            policy.WithOrigins(allowedOrigins)
                 .AllowAnyMethod()
                 .AllowAnyHeader()
                 .AllowCredentials();
       }
    });
});

var app = builder.Build();

// DB Migration + Seeding
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    await DbSeeder.SeedAsync(scope.ServiceProvider);
}

// Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bike360 API v1");
        c.RoutePrefix = "swagger";
    });
}
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

//  Middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors("AllowFrontend");
app.UseAuthentication(); // (add if using JWT)
app.UseAuthorization();
app.MapControllers();

app.Run();