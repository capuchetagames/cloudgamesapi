using CloudGamesApi.Middlewares;
using CloudGamesApi.Service;
using CloudGamesApi.Service.Validator;
using Core.Models;
using Core.Repository;
using FluentValidation;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json").Build();

var connectionString = configuration.GetConnectionString("ConnectionString");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
},ServiceLifetime.Scoped);

builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddMemoryCache();
builder.Services.AddTransient<ICacheService, MemCacheService>();

builder.Services.AddTransient<ICorrelationIdService, CorrelationIdService>();


builder.Services.AddControllers();
builder.Services.AddValidatorsFromAssemblyContaining<UserValidator>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerDocumentation();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient(typeof(BaseLogger<>));

builder.AddJwtAuthentication();
builder.Services.AddPolicyAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseLogMiddleware();

app.UseAuthorization();

app.MapControllers();

app.UseLogMiddleware();

app.Run();