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
using AgencyPlatform.Application.Authorization.Requirements;
using AgencyPlatform.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using QuestPDF.Infrastructure;
using AgencyPlatform.API.Hubs;
using AgencyPlatform.API.Utils;
using AgencyPlatform.Application.Interfaces.Utils;
using AgencyPlatform.Application.Interfaces.Services.PagoVerificacion;
using AgencyPlatform.Infrastructure.Services.PagoVerificacion;

var builder = WebApplication.CreateBuilder(args);
QuestPDF.Settings.License = LicenseType.Community;


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
            // Para API REST
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

            // Si el encabezado existe pero no comienza con "Bearer "
            // (usuario ha pasado solamente el token sin el prefijo)
            if (!string.IsNullOrEmpty(authHeader) && !authHeader.StartsWith("Bearer "))
            {
                // Establecer directamente el token para la validación
                context.Token = authHeader;
            }

            // Para SignalR - permitir token en query string
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) &&
                path.StartsWithSegments("/api/hubs"))
            {
                context.Token = accessToken;
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
builder.Services.AddScoped<ISolicitudAgenciaRepository, SolicitudAgenciaRepository>();
builder.Services.AddScoped<IComisionRepository, ComisionRepository>();

// Registrar servicios
builder.Services.AddScoped<IAcompananteService, AcompananteService>();
builder.Services.AddScoped<IAgenciaService, AgenciaService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IArchivosService, ArchivosService>();
builder.Services.AddScoped<IPagoVerificacionService, PagoVerificacionService>();


// Repositorios adicionales
builder.Services.AddScoped<IFotoRepository, FotoRepository>();
builder.Services.AddScoped<IServicioRepository, ServicioRepository>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IVisitaRepository, VisitaRepository>();
builder.Services.AddScoped<IContactoRepository, ContactoRepository>();
builder.Services.AddScoped<IVisitaPerfilRepository, VisitaPerfilRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ISolicitudRegistroAgenciaRepository, SolicitudRegistroAgenciaRepository>();
// En Startup.cs o el lugar donde configuras tus servicios
builder.Services.AddScoped<IPagoVerificacionRepository, PagoVerificacionRepository>();


// Notificaciones
builder.Services.AddScoped<INotificadorRealTime, NotificadorSignalR>();

// DTO para registro de contactos
builder.Services.AddScoped<RegistrarContactoDto>();

// Agregar HttpContextAccessor para acceder al usuario actual
builder.Services.AddHttpContextAccessor();

// Configurar SignalR con opciones
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
    options.MaximumReceiveMessageSize = 102400; // 100 KB
});

// 🌐 CORS (permitir frontend local)
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendDev", policy =>
    {
        policy.WithOrigins(
            "http://localhost:5173",
            "http://127.0.0.1:5500"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

// Políticas de autorización
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AgenciaOwnerOnly", policy =>
        policy.Requirements.Add(new EsDuenoAgenciaRequirement()));
});

builder.Services.AddScoped<IAuthorizationHandler, EsDuenoAgenciaHandler>();

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

// Mapear el hub de SignalR (solo una ruta)
app.MapHub<NotificacionesHub>("/api/Hubs/notificaciones");

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