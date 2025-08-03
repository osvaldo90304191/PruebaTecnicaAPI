using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims; // AuthController
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();//controladores API.
builder.Services.AddEndpointsApiExplorer();
// Configuración de Swagger para que reconozca JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PruebaTecnicaAPI", Version = "v1" });

    // Definición de la seguridad para usar JWT Bearer
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingresa el token JWT con el prefijo 'Bearer '."
    });

    // Requisito de seguridad para los endpoints
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

// --- INICIO DE CONFIGURACIÓN DE SERVICIOS JWT ---
// Aquí Configuro cómo la aplicación manejará los tokens JWT.
var secretKey = builder.Configuration["Jwt:Key"] ?? "EstaEsUnaClaveSuperSecretaParaTuPruebaJWTDeBackendCon.NETCoreYDebeSerLoSuficientementeLargaParaSerSegura"; // Lee la clave de configuración o usa un fallback

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false, // Para esta prueba, no valido el emisor
            ValidateAudience = false, // Para esta prueba, no valido la audiencia
            ValidateLifetime = true, // Es FUNDAMENTAL validar la vigencia del token
            ValidateIssuerSigningKey = true, // Es FUNDAMENTAL validar la clave de firma
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

builder.Services.AddAuthorization(); // Agrego el servicio de autorización, permite usar [Authorize]
// --- FIN DE CONFIGURACIÓN DE SERVICIOS JWT ---

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// --- INICIO DE MIDDLEWARES JWT ---
app.UseAuthentication();
app.UseAuthorization();
// --- FIN DE MIDDLEWARES JWT ---

app.MapControllers(); // Este mapea los controladores para que respondan a las rutas.

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
