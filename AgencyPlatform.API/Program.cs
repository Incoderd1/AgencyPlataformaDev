using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AgencyPlatform.Application.Interfaces;
using AgencyPlatform.Application.Interfaces.Repositories;
using AgencyPlatform.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore;
using AgencyPlatform.Application.Interfaces.Services;
using AgencyPlatform.Core.Entities;
using AgencyPlatform.Application.Services;
using AgencyPlatform.Infrastructure.Services.Email;
using Microsoft.OpenApi.Models;
using AgencyPlatform.Application.Middleware;
using AgencyPlatform.Application.MapperProfiles;
using AgencyPlatform.Application.Interfaces.Services.Agencias;
using AgencyPlatform.Infrastructure.Mappers;
using AgencyPlatform.Infrastructure.Services.Agencias;
using AgencyPlatform.Application.Interfaces.Services.Acompanantes;
using AgencyPlatform.Infrastructure.Services.Acompanantes;
using AgencyPlatform.Application.Interfaces.Services.Categoria;
using AgencyPlatform.Infrastructure.Services.Categoria;
using AgencyPlatform.Application.Interfaces.Repositories.Archivos;
using AgencyPlatform.Application.DTOs;

var builder = WebApplication.CreateBuilder(args);

// 📦 Cargar configuración JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");

// 📂 Configurar DbContext PostgreSQL
builder.Services.AddDbContext<AgencyPlatformDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔐 Configurar Autenticación JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

            // Si el encabezado existe pero no comienza con "Bearer "
            // (usuario ha pasado solamente el token sin el prefijo)
            if (!string.IsNullOrEmpty(authHeader) && !authHeader.StartsWith("Bearer "))
            {
                // Establecer directamente el token para la validación
                context.Token = authHeader;
            }

            return Task.CompletedTask;
        }
    };

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!))
    };
});

// 💉 Inyección de dependencias
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEmailSender, EmailSender>();


// ✅ FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<UserService>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();


//builder.Services.AddAutoMapper(typeof(AgenciasProfile).Assembly);
builder.Services.AddAutoMapper(typeof(AgenciasProfile).Assembly);

builder.Services.AddAutoMapper(typeof(MappingProfile));

// Registrar repositorios
builder.Services.AddScoped<IAgenciaRepository, AgenciaRepository>();
builder.Services.AddScoped<IAcompananteRepository, AcompananteRepository>();
builder.Services.AddScoped<IVerificacionRepository, VerificacionRepository>();
builder.Services.AddScoped<IAnuncioDestacadoRepository, AnuncioDestacadoRepository>();
builder.Services.AddScoped<IIntentoLoginRepository, IntentoLoginRepository>();

builder.Services.AddScoped<IAcompananteService, AcompananteService>();
builder.Services.AddScoped<IAcompananteRepository, AcompananteRepository>();

// En Program.cs o Startup.cs
builder.Services.AddScoped<IFotoRepository, FotoRepository>();
builder.Services.AddScoped<IServicioRepository, ServicioRepository>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IVisitaRepository, VisitaRepository>();
builder.Services.AddScoped<IContactoRepository, ContactoRepository>();

builder.Services.AddScoped<IVisitaPerfilRepository, VisitaPerfilRepository>();
builder.Services.AddScoped<IContactoRepository, ContactoRepository>();
builder.Services.AddScoped<IFotoRepository, FotoRepository>();
builder.Services.AddScoped<IServicioRepository, ServicioRepository>();

// Servicios
builder.Services.AddScoped<IArchivosService, ArchivosService>();

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IArchivosService, ArchivosService>();

// DTO para registro de contactos
builder.Services.AddScoped<RegistrarContactoDto>();



// Registrar servicios
builder.Services.AddScoped<IAgenciaService, AgenciaService>();
// Registrar repositorios
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();

// Registrar servicios
builder.Services.AddScoped<ICategoriaService, CategoriaService>();

// Agregar HttpContextAccessor para acceder al usuario actual
builder.Services.AddHttpContextAccessor();

// 🌐 CORS (permitir frontend local)
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendDev", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// 🧪 Swagger con JWT
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "AgencyPlatform API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Pega solo el token (sin escribir 'Bearer '). El sistema lo agregará automáticamente 🔐",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
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
            Array.Empty<string>()
        }
    });
});

// 📦 Controllers
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// 🧰 Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ApiExceptionMiddleware>();
app.UseCors("FrontendDev");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();