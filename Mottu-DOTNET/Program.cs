using Microsoft.EntityFrameworkCore;
using Mottu_DOTNET.src.Application.Interfaces;
using Mottu_DOTNET.src.Application.Services;
using Mottu_DOTNET.src.Infrastructure.Data;
using Mottu_DOTNET.src.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// --------------------
// Serviços principais
// --------------------
builder.Services.AddControllers();
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.ReportApiVersions = true; // Retorna headers como api-supported-versions
    options.ApiVersionReader = new Microsoft.AspNetCore.Mvc.Versioning.HeaderApiVersionReader("x-api-version");
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --------------------
// Banco de dados
// --------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --------------------
// Injeção de dependências (Repositories e Services)
// --------------------
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IMotoRepository, MotoRepository>();
builder.Services.AddScoped<IPatioRepository, PatioRepository>();
builder.Services.AddScoped<PatioService>();

// --------------------
// Configuração de CORS
// --------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policy =>
        policy.WithOrigins("http://10.3.63.34:19000", "http://10.3.63.34:19001")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// --------------------
// Implementando o Health Check
// --------------------

builder.Services.AddHealthChecks();

var app = builder.Build();



// --------------------
// Swagger (sempre ativo, em qualquer ambiente)
// --------------------
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "DEVOPS5 API V1");
});

// Redireciona a raiz (/) para a página do Swagger
app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger/index.html");
    return Task.CompletedTask;
});

// --------------------
app.MapHealthChecks("/health");
// --------------------

// --------------------
// Pipeline HTTP
// --------------------
app.UseHttpsRedirection();

app.UseCors("AllowLocalhost");

app.UseAuthorization();

app.MapControllers();

app.Run();
