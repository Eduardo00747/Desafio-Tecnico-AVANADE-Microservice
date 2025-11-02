using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Estoque.API.Data;
using Estoque.API.Repositories;
using Estoque.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuration - appsettings.json terá ConnectionStrings e JWT settings
var configuration = builder.Configuration;

// Configuração do EF Core com MySQL
builder.Services.AddDbContext<EstoqueDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(9, 0, 0)) // coloque aqui a versão do seu MySQL
    ));

// Repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// RabbitMQ Consumer
builder.Services.AddHostedService<RabbitMQConsumer>();

// Authentication - JWT
var jwtSettings = configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "ECommerceAuth",
        ValidAudience = "ECommerceClients",
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes("MinhaChaveSuperSecreta_DeveTerPeloMenos16Chars!")            )
    };
});

builder.Services.AddSwaggerGen(options =>
{

    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Estoque API",
        Version = "v1"
    });

    var securitySchema = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Insira o token JWT desta forma: Bearer {seu token}",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    };

    options.AddSecurityDefinition("Bearer", securitySchema);

    var securityRequirement = new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    };

    options.AddSecurityRequirement(securityRequirement);
});

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

app.Run();
