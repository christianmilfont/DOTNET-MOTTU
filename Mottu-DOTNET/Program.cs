using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Mottu_DOTNET.src.Application.Interfaces;
using Mottu_DOTNET.src.Application.Services;
using Mottu_DOTNET.src.Application.Services.Auth;
using Mottu_DOTNET.src.Infrastructure.Data;
using Mottu_DOTNET.src.Infrastructure.Repositories;
using Mottu_DOTNET.src.Domain.ML;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --------------------
// Serviços principais
// --------------------
builder.Services.AddControllers();
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.ApiVersionReader = new Microsoft.AspNetCore.Mvc.Versioning.HeaderApiVersionReader("x-api-version");
});

builder.Services.AddEndpointsApiExplorer();

// --------------------
// Configuração do Swagger com JWT
// --------------------
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "DEVOPS5 API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Insira o token JWT com o prefixo 'Bearer ' (exemplo: 'Bearer abcde12345')",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            }, new string[] {}
        }
    });
});

// --------------------
// Banco de dados (condicional para testes)
// --------------------
if (builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase("TestDb"));
}
else
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}

// --------------------
// Injeção de dependências (Repositories e Services)
// --------------------
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IMotoRepository, MotoRepository>();
builder.Services.AddScoped<IPatioRepository, PatioRepository>();
builder.Services.AddScoped<PatioService>();
builder.Services.AddScoped<JwtService>();

// --------------------
// Injeção de dependências para ML.NET
// --------------------
builder.Services.AddSingleton<TextClassificationModel>();
builder.Services.AddScoped<TrainingService>();

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
// Health Checks
// --------------------
builder.Services.AddHealthChecks();

// --------------------
// Configuração do JWT
// --------------------
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

var app = builder.Build();

// --------------------
// Pipeline HTTP
// --------------------
app.UseHttpsRedirection();

app.UseCors("AllowLocalhost");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "DEVOPS5 API V1");
});

app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger/index.html");
    return Task.CompletedTask;
});

app.MapHealthChecks("/health");

app.Run();

// --------------------
// Permite testes de integração
// --------------------
namespace Mottu_DOTNET
{
    public partial class Program { }
}
