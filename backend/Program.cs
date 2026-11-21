using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using backend.Auth;
using backend.Data;
using backend.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// EF Core + Npgsql (ЛР10). Строка подключения читается по ключу
// ConnectionStrings:Default; настоящий пароль задаётся через user-secrets,
// в Git он не попадает.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// DI (ЛР7): контроллер не создаёт сервис через new — реализацию
// хранения можно заменить, не переписывая контроллер.
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<JwtTokenService>();

// Безопасный 500 (ЛР8): одна строка в логе, клиенту — ProblemDetails без стека
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Вход (ЛР11): один способ — учебный JWT в заголовке Authorization.
var jwtSection = builder.Configuration.GetSection("Jwt");
var signingKey = AuthOptions.GetSigningKey(builder.Configuration);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSection["Issuer"] ?? AuthOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSection["Audience"] ?? AuthOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
            ValidateLifetime = true
        };

        // Гость — 401, чужое действие — 403; ответы в виде ProblemDetails,
        // чтобы их было видно в Swagger и Network
        options.Events = new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new
                {
                    status = 401,
                    title = "Требуется вход"
                });
            },
            OnForbidden = async context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new
                {
                    status = 403,
                    title = "Недостаточно прав для этого действия"
                });
            }
        };
    });

builder.Services.AddAuthorization();

// CORS для сквозной сборки (ЛР12): конкретный origin, не "*".
const string frontendOrigin = "http://localhost:5173";
builder.Services.AddCors(options => options.AddPolicy("Frontend", policy =>
    policy.WithOrigins(frontendOrigin)
        .AllowAnyHeader()
        .AllowAnyMethod()));

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Frontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Служебные адреса ЛР7 (Minimal API допустим только для /health и /info,
// главный ресурс — контроллеры). /health — только доступность процесса
// (liveness), создание обращений он не проверяет.
app.MapGet("/health", () => Results.Text("ok"));

app.MapGet("/info", (IHostEnvironment environment) => Results.Json(new
{
    application = "backend",
    project = "Система обработки обращений жильцов общежития",
    environment = environment.EnvironmentName,
    lab = "ЛР7"
}));

app.Run();
