using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RealtorConnect.Data;
using RealtorConnect.Hubs;
using RealtorConnect.Models;
using RealtorConnect.Repositories;
using RealtorConnect.Repositories.Interfaces;
using RealtorConnect.Services;
using RealtorConnect.Services.Interfaces;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Настройка сериализации JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// Добавляем сервисы в контейнер DI
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Настройка CORS для фронтенда (React на порту 3000)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Настройка SignalR для чата
builder.Services.AddSignalR();

// Настройка подключения к базе данных (PostgreSQL)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Регистрация репозиториев в DI
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IRealtorRepository, RealtorRepository>();
builder.Services.AddScoped<IApartmentRepository, ApartmentRepository>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();

// Регистрация сервисов в DI
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IApartmentService, ApartmentService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IRealtorService, RealtorService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IFavoriteService, FavoriteService>();

// Настройка аутентификации с помощью JWT
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("JWT Key is not configured in appsettings.json.");
}
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
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

// Настройка Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "RealtorConnect", Version = "v1" });

    // Добавляем поддержку JWT-аутентификации в Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
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
            },
            new string[] {}
        }
    });
});



var app = builder.Build();

// Настройка конвейера обработки запросов
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "RealtorConnect v1");
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Для доступа к файлам в wwwroot (например, загруженным фото)
app.UseCors("AllowReact");
app.UseAuthentication();
app.UseAuthorization();

app.MapHub<ChatHub>("/chatHub"); // Маршрут для SignalR
app.MapControllers();

app.Run();