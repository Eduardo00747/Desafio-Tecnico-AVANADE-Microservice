using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;
using System.Text;
using Vendas.API.Data;
using Vendas.API.Repositories;
using Vendas.API.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuration (appsettings.json should conter conexões)
var configuration = builder.Configuration;

// DbContext MySQL
var conn = configuration.GetConnectionString("VendasMySql");
builder.Services.AddDbContext<VendasDbContext>(opt =>
    opt.UseMySql(conn, ServerVersion.AutoDetect(conn)));

// Repositories
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// HttpClient para Estoque API
builder.Services.AddHttpClient<IEstoqueClient, EstoqueClient>(client =>
{
    var estoqueBaseUrl = configuration["EstoqueApi:BaseUrl"];
    if (string.IsNullOrEmpty(estoqueBaseUrl))
        throw new InvalidOperationException("A configuração 'EstoqueApi:BaseUrl' não foi encontrada.");

    client.BaseAddress = new Uri(estoqueBaseUrl);
});

// RabbitMQ
builder.Services.AddSingleton<IConnectionFactory>(sp =>
{
    var cfg = configuration.GetSection("RabbitMQ");
    return new ConnectionFactory
    {
        HostName = cfg["Host"],
        UserName = cfg["UserName"],
        Password = cfg["Password"]
    };
});
builder.Services.AddSingleton<IRabbitMQService, RabbitMQService>();

// JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;

    var jwtKey = configuration["Jwt:Key"];
    if (string.IsNullOrEmpty(jwtKey))
        throw new InvalidOperationException("A chave JWT ('Jwt:Key') não foi configurada.");

    options.TokenValidationParameters = new TokenValidationParameters
    {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = "ECommerceAuth",
            ValidAudience = "ECommerceClients",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("MinhaChaveSuperSecreta_DeveTerPeloMenos16Chars!")
            )
    };
});

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Vendas.API",
        Version = "v1"
    });

    // 🔐 Configuração para mostrar o botão Authorize (JWT)
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT no campo abaixo (exemplo: Bearer eyJhbGciOiJIUzI1NiIsInR5c...)"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

app.Run();
