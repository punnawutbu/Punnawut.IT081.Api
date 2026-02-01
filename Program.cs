using Punnawut.IT081.Api.Shared.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddCors(o =>
{
    o.AddPolicy("fe", p => p
        .WithOrigins("http://localhost:3000")
        .AllowAnyHeader()
        .AllowAnyMethod());
});

builder.Services.AddIT081Repositories();

var app = builder.Build();

// Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("fe");

app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { ok = true }))
   .WithName("Health")
   .WithOpenApi();

app.Run();
