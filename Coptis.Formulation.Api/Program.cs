
using Coptis.Formulation.Infrastructure.Background;
using Coptis.Formulation.Infrastructure.FileWatching;
using Coptis.Formulation.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Coptis.Formulation.Application.Abstractions.Repositories;
using Coptis.Formulation.Application.Abstractions.Services;
using Coptis.Formulation.Application.Implementations.Services;
using Coptis.Formulation.Domain.Services;
using Coptis.Formulation.Infrastructure.Repositories;
using Coptis.Formulation.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(
    builder.Configuration.GetConnectionString("Default")));

// Import Configuration
builder.Services.Configure<ImportOptions>(builder.Configuration.GetSection("Import"));
builder.Services.AddHostedService<AutoImportHostedService>();

// Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Domain Services
builder.Services.AddSingleton<IPercentageNormalizer, PercentageNormalizer>();

// Application Services
builder.Services.AddScoped<IFormulaValidationService, FormulaValidationService>();
builder.Services.AddScoped<IFormulaImportService, FormulaImportService>();
builder.Services.AddScoped<IFormulaService, FormulaService>();
builder.Services.AddScoped<IRawMaterialService, RawMaterialService>();
builder.Services.AddScoped<ISubstanceService, SubstanceService>();

// Repositories
builder.Services.AddScoped<IFormulaRepository, FormulaRepository>();
builder.Services.AddScoped<IRawMaterialRepository, RawMaterialRepository>();
builder.Services.AddScoped<ISubstanceRepository, SubstanceRepository>();

// API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.WithOrigins("https://localhost:7163")
     .AllowAnyHeader()
     .AllowAnyMethod()
     .AllowCredentials()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.MapControllers();
app.Run();